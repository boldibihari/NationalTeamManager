using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using NationalTeamManager.DataImporter.Models;
using NationalTeamManager.DataImporter.Providers.SofaScore;
using NationalTeamManager.DataImporter.Providers.SofaScore.Models;
using NationalTeamManager.Domain.Entities;
using NationalTeamManager.Infrastructure.Persistence;

namespace NationalTeamManager.DataImporter.Importers
{
    public class MatchIncidentImporter(
        ISofaScoreClient sofaScoreClient,
        NationalTeamManagerDbContext dbContext
    )
    {
        private const string DataSource = "SofaScore";

        public async Task ImportAsync(CancellationToken cancellationToken = default)
        {
            var matches = await dbContext
                .Matches.Where(x => x.DataSource == DataSource)
                .ToListAsync(cancellationToken);

            var existingIncidents = await dbContext
                .MatchIncidents.Where(x => x.DataSource == DataSource)
                .ToListAsync(cancellationToken);

            var incidentsByExternalKey = existingIncidents.ToDictionary(x =>
                (x.ExternalKey, x.DataSource!)
            );
            var usedExistingIncidentIds = new HashSet<int>();
            var incidentStatistics = new ImportStatistics();
            var processedMatchCount = 0;
            var skippedMatchIdCount = 0;
            var skippedMissingStatusCount = 0;
            var skippedUnfinishedStatusCount = 0;

            foreach (var match in matches)
            {
                cancellationToken.ThrowIfCancellationRequested();

                if (
                    !int.TryParse(
                        match.ExternalId,
                        NumberStyles.None,
                        CultureInfo.InvariantCulture,
                        out var externalMatchId
                    )
                )
                {
                    skippedMatchIdCount++;
                    continue;
                }

                if (string.IsNullOrWhiteSpace(match.StatusType))
                {
                    skippedMissingStatusCount++;
                    continue;
                }

                if (
                    !string.Equals(match.StatusType, "finished", StringComparison.OrdinalIgnoreCase)
                )
                {
                    skippedUnfinishedStatusCount++;
                    continue;
                }

                var response = await sofaScoreClient.GetMatchIncidentsAsync(
                    externalMatchId,
                    cancellationToken
                );
                processedMatchCount++;

                var incidentsWithKeys = response
                    .Incidents.Where(x => x.Id is not null)
                    .Select(x =>
                        (Source: x, ExternalKey: GenerateExternalKey(match.ExternalId!, x))
                    )
                    .Concat(
                        response
                            .Incidents.Where(x => x.Id is null)
                            .GroupBy(
                                x => GenerateExternalKey(match.ExternalId!, x),
                                StringComparer.Ordinal
                            )
                            .SelectMany(group =>
                                group
                                    .OrderBy(GetExternalKeySortValue, StringComparer.Ordinal)
                                    .Select(
                                        (source, occurrence) =>
                                            (
                                                Source: source,
                                                ExternalKey: AddCollisionSuffix(
                                                    group.Key,
                                                    occurrence
                                                )
                                            )
                                    )
                            )
                    );

                foreach (var (source, externalKey) in incidentsWithKeys)
                {
                    var externalId = source.Id?.ToString(CultureInfo.InvariantCulture);

                    if (
                        incidentsByExternalKey.TryGetValue(
                            (externalKey, DataSource),
                            out var incident
                        )
                    )
                    {
                        var hasChanges = UpdateIncident(
                            incident,
                            match,
                            source,
                            externalKey,
                            externalId
                        );
                        incidentStatistics.Add(
                            hasChanges ? ImportResult.Updated : ImportResult.Unchanged
                        );
                    }
                    else if (source.Id is null)
                    {
                        incident = existingIncidents.FirstOrDefault(x =>
                            !usedExistingIncidentIds.Contains(x.Id)
                            && MatchesExistingIncident(x, match, source)
                        );

                        if (incident is not null)
                        {
                            incidentsByExternalKey.Remove((incident.ExternalKey, DataSource));
                            incidentsByExternalKey.Add((externalKey, DataSource), incident);

                            var hasChanges = UpdateIncident(
                                incident,
                                match,
                                source,
                                externalKey,
                                externalId
                            );
                            incidentStatistics.Add(
                                hasChanges ? ImportResult.Updated : ImportResult.Unchanged
                            );
                        }
                    }

                    if (incident is not null)
                    {
                        usedExistingIncidentIds.Add(incident.Id);
                    }
                    else
                    {
                        incident = CreateIncident(match, source, externalKey, externalId);
                        dbContext.MatchIncidents.Add(incident);
                        incidentsByExternalKey.Add((externalKey, DataSource), incident);
                        incidentStatistics.Add(ImportResult.Created);
                    }
                }
            }

            await dbContext.SaveChangesAsync(cancellationToken);

            Console.WriteLine(
                $"""
                Mérkőzésesemény-import kész:

                  Mérkőzések:
                    Feldolgozott: {processedMatchCount}
                    Kihagyott, hiányzó vagy érvénytelen SofaScore azonosítóval: {skippedMatchIdCount}
                    Kihagyott, hiányzó státusszal: {skippedMissingStatusCount}
                    Kihagyott, nem befejezett státusszal: {skippedUnfinishedStatusCount}

                  Események:
                    Új: {incidentStatistics.Created}
                    Frissített: {incidentStatistics.Updated}
                    Változatlan: {incidentStatistics.Unchanged}
                """
            );
        }

        private static MatchIncident CreateIncident(
            Match match,
            SofaScoreIncident source,
            string externalKey,
            string? externalId
        )
        {
            var incident = new MatchIncident
            {
                Match = match,
                ExternalKey = externalKey,
                ExternalId = externalId,
                DataSource = DataSource,
            };
            UpdateIncident(incident, match, source, externalKey, externalId);
            return incident;
        }

        private static bool UpdateIncident(
            MatchIncident incident,
            Match match,
            SofaScoreIncident source,
            string externalKey,
            string? externalId
        )
        {
            var playerExternalId = ToExternalId(source.Player?.Id);
            var assistPlayerExternalId = ToExternalId(source.Assist1?.Id);
            var playerInExternalId = ToExternalId(source.PlayerIn?.Id);
            var playerOutExternalId = ToExternalId(source.PlayerOut?.Id);
            var hasChanges =
                incident.MatchId != match.Id
                || incident.ExternalKey != externalKey
                || incident.ExternalId != externalId
                || incident.IncidentType != source.IncidentType
                || incident.IncidentClass != source.IncidentClass
                || incident.Time != source.Time
                || incident.AddedTime != source.AddedTime
                || incident.ReversedPeriodTime != source.ReversedPeriodTime
                || incident.IsHome != source.IsHome
                || incident.PlayerExternalId != playerExternalId
                || incident.PlayerName != source.Player?.Name
                || incident.AssistPlayerExternalId != assistPlayerExternalId
                || incident.AssistPlayerName != source.Assist1?.Name
                || incident.PlayerInExternalId != playerInExternalId
                || incident.PlayerInName != source.PlayerIn?.Name
                || incident.PlayerOutExternalId != playerOutExternalId
                || incident.PlayerOutName != source.PlayerOut?.Name
                || incident.HomeScore != source.HomeScore
                || incident.AwayScore != source.AwayScore
                || incident.Reason != source.Reason
                || incident.Rescinded != source.Rescinded
                || incident.Text != source.Text
                || incident.Description != source.Description;

            incident.Match = match;
            incident.ExternalKey = externalKey;
            incident.ExternalId = externalId;
            incident.IncidentType = source.IncidentType;
            incident.IncidentClass = source.IncidentClass;
            incident.Time = source.Time;
            incident.AddedTime = source.AddedTime;
            incident.ReversedPeriodTime = source.ReversedPeriodTime;
            incident.IsHome = source.IsHome;
            incident.PlayerExternalId = playerExternalId;
            incident.PlayerName = source.Player?.Name;
            incident.AssistPlayerExternalId = assistPlayerExternalId;
            incident.AssistPlayerName = source.Assist1?.Name;
            incident.PlayerInExternalId = playerInExternalId;
            incident.PlayerInName = source.PlayerIn?.Name;
            incident.PlayerOutExternalId = playerOutExternalId;
            incident.PlayerOutName = source.PlayerOut?.Name;
            incident.HomeScore = source.HomeScore;
            incident.AwayScore = source.AwayScore;
            incident.Reason = source.Reason;
            incident.Rescinded = source.Rescinded;
            incident.Text = source.Text;
            incident.Description = source.Description;

            return hasChanges;
        }

        private static string? ToExternalId(int? id) => id?.ToString(CultureInfo.InvariantCulture);

        private static string GenerateExternalKey(string matchExternalId, SofaScoreIncident source)
        {
            if (source.Id is not null)
            {
                return $"id:{source.Id.Value.ToString(CultureInfo.InvariantCulture)}";
            }

            var identity = GetExternalKeySortValue(matchExternalId, source);
            var hash = SHA256.HashData(Encoding.UTF8.GetBytes(identity));

            return $"generated:{Convert.ToHexString(hash)}";
        }

        private static string GetExternalKeySortValue(SofaScoreIncident source) =>
            GetExternalKeySortValue(string.Empty, source);

        private static string GetExternalKeySortValue(
            string matchExternalId,
            SofaScoreIncident source
        )
        {
            return JsonSerializer.Serialize(
                new
                {
                    MatchExternalId = matchExternalId,
                    source.IncidentType,
                    source.IncidentClass,
                    source.Time,
                    source.AddedTime,
                    source.Length,
                    source.TimeSeconds,
                    source.PeriodTimeSeconds,
                    source.ReversedPeriodTime,
                    source.ReversedPeriodTimeSeconds,
                    source.IsHome,
                    Player = new { Id = source.Player?.Id, Name = source.Player?.Name },
                    Assist1 = new { Id = source.Assist1?.Id, Name = source.Assist1?.Name },
                    PlayerIn = new { Id = source.PlayerIn?.Id, Name = source.PlayerIn?.Name },
                    PlayerOut = new { Id = source.PlayerOut?.Id, Name = source.PlayerOut?.Name },
                    source.HomeScore,
                    source.AwayScore,
                    source.Reason,
                    source.Rescinded,
                    source.Text,
                    source.Description,
                }
            );
        }

        private static string AddCollisionSuffix(string externalKey, int occurrence)
        {
            return occurrence == 0 ? externalKey : $"{externalKey}:duplicate:{occurrence}";
        }

        private static bool MatchesExistingIncident(
            MatchIncident incident,
            Match match,
            SofaScoreIncident source
        )
        {
            return incident.MatchId == match.Id
                && incident.ExternalId is null
                && incident.IncidentType == source.IncidentType
                && incident.IncidentClass == source.IncidentClass
                && incident.Time == source.Time
                && incident.AddedTime == source.AddedTime
                && incident.ReversedPeriodTime == source.ReversedPeriodTime
                && incident.IsHome == source.IsHome
                && incident.PlayerExternalId == ToExternalId(source.Player?.Id)
                && incident.PlayerName == source.Player?.Name
                && incident.AssistPlayerExternalId == ToExternalId(source.Assist1?.Id)
                && incident.AssistPlayerName == source.Assist1?.Name
                && incident.PlayerInExternalId == ToExternalId(source.PlayerIn?.Id)
                && incident.PlayerInName == source.PlayerIn?.Name
                && incident.PlayerOutExternalId == ToExternalId(source.PlayerOut?.Id)
                && incident.PlayerOutName == source.PlayerOut?.Name
                && incident.HomeScore == source.HomeScore
                && incident.AwayScore == source.AwayScore
                && incident.Reason == source.Reason
                && incident.Rescinded == source.Rescinded
                && incident.Text == source.Text
                && incident.Description == source.Description;
        }
    }
}
