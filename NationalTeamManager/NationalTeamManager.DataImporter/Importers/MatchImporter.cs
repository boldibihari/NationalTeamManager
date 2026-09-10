using Microsoft.EntityFrameworkCore;
using NationalTeamManager.DataImporter.Models;
using NationalTeamManager.DataImporter.Providers.SofaScore;
using NationalTeamManager.DataImporter.Providers.SofaScore.Models;
using NationalTeamManager.Domain.Entities;
using NationalTeamManager.Infrastructure.Persistence;

namespace NationalTeamManager.DataImporter.Importers
{
    public class MatchImporter(
        ISofaScoreClient sofaScoreClient,
        NationalTeamManagerDbContext dbContext
    )
    {
        private const string DataSource = "SofaScore";

        public async Task ImportAsync(int teamId, CancellationToken cancellationToken = default)
        {
            var lastMatches = await sofaScoreClient.GetLastMatchesAsync(teamId, cancellationToken);

            var nextMatches = await sofaScoreClient.GetNextMatchesAsync(teamId, cancellationToken);

            var matches = lastMatches
                .Events.Concat(nextMatches.Events)
                .GroupBy(x => x.Id)
                .Select(x => x.First())
                .ToList();

            var externalIds = matches.Select(x => x.Id.ToString()).Distinct().ToList();

            var existingMatches = await dbContext
                .Matches.Where(x =>
                    x.DataSource == DataSource
                    && x.ExternalId != null
                    && externalIds.Contains(x.ExternalId)
                )
                .ToListAsync(cancellationToken);

            var teamExternalIds = matches
                .SelectMany(x => new[] { x.HomeTeam, x.AwayTeam })
                .Select(x => x.Id.ToString())
                .Distinct()
                .ToList();

            var existingTeams = await dbContext
                .Teams.Where(x =>
                    x.DataSource == DataSource
                    && x.ExternalId != null
                    && teamExternalIds.Contains(x.ExternalId)
                )
                .ToListAsync(cancellationToken);

            var competitionExternalIds = matches
                .Where(x => x.Tournament?.UniqueTournament is not null)
                .Select(x => x.Tournament!.UniqueTournament!.Id.ToString())
                .Distinct()
                .ToList();

            var existingCompetitions = await dbContext
                .Competitions.Where(x =>
                    x.DataSource == DataSource
                    && x.ExternalId != null
                    && competitionExternalIds.Contains(x.ExternalId)
                )
                .ToListAsync(cancellationToken);

            var editionExternalIds = matches
                .Where(x => x.Season is not null)
                .Select(x => x.Season!.Id.ToString())
                .Distinct()
                .ToList();

            var existingEditions = await dbContext
                .CompetitionEditions.Where(x =>
                    x.DataSource == DataSource
                    && x.ExternalId != null
                    && editionExternalIds.Contains(x.ExternalId)
                )
                .ToListAsync(cancellationToken);

            var stageExternalIds = matches
                .Where(x => x.Season is not null && x.RoundInfo?.Round is not null)
                .Select(x => $"{x.Season!.Id}_round_{x.RoundInfo!.Round}")
                .Distinct()
                .ToList();

            var existingStages = await dbContext
                .CompetitionStages.Where(x =>
                    x.DataSource == DataSource
                    && x.ExternalId != null
                    && stageExternalIds.Contains(x.ExternalId)
                )
                .ToListAsync(cancellationToken);

            var teamsByExternalId = existingTeams.ToDictionary(
                x => x.ExternalId!,
                StringComparer.Ordinal
            );

            var competitionsByExternalId = existingCompetitions.ToDictionary(
                x => x.ExternalId!,
                StringComparer.Ordinal
            );

            var editionsByExternalId = existingEditions.ToDictionary(
                x => x.ExternalId!,
                StringComparer.Ordinal
            );

            var stagesByExternalId = existingStages.ToDictionary(
                x => x.ExternalId!,
                StringComparer.Ordinal
            );

            var matchesByExternalId = existingMatches.ToDictionary(
                x => x.ExternalId!,
                StringComparer.Ordinal
            );

            var matchStatistics = new ImportStatistics();
            var teamStatistics = new ImportStatistics();
            var competitionStatistics = new ImportStatistics();
            var editionStatistics = new ImportStatistics();
            var stageStatistics = new ImportStatistics();

            var countedTeamExternalIds = new HashSet<string>(StringComparer.Ordinal);

            var countedCompetitionExternalIds = new HashSet<string>(StringComparer.Ordinal);

            var countedEditionExternalIds = new HashSet<string>(StringComparer.Ordinal);

            var countedStageExternalIds = new HashSet<string>(StringComparer.Ordinal);

            foreach (var source in matches)
            {
                var result = ImportMatch(
                    source,
                    teamsByExternalId,
                    competitionsByExternalId,
                    editionsByExternalId,
                    stagesByExternalId,
                    matchesByExternalId,
                    teamStatistics,
                    competitionStatistics,
                    editionStatistics,
                    stageStatistics,
                    countedTeamExternalIds,
                    countedCompetitionExternalIds,
                    countedEditionExternalIds,
                    countedStageExternalIds
                );

                matchStatistics.Add(result);
            }

            await dbContext.SaveChangesAsync(cancellationToken);

            Console.WriteLine(
                $"""
                Meccsimport kész:

                  Mérkőzések:
                    Összesen: {matches.Count}
                    Új: {matchStatistics.Created}
                    Frissített: {matchStatistics.Updated}
                    Változatlan: {matchStatistics.Unchanged}

                  Csapatok:
                    Új: {teamStatistics.Created}
                    Frissített: {teamStatistics.Updated}
                    Változatlan: {teamStatistics.Unchanged}

                  Versenyek:
                    Új: {competitionStatistics.Created}
                    Frissített: {competitionStatistics.Updated}
                    Változatlan: {competitionStatistics.Unchanged}

                  Szezonok:
                    Új: {editionStatistics.Created}
                    Frissített: {editionStatistics.Updated}
                    Változatlan: {editionStatistics.Unchanged}

                  Szakaszok:
                    Új: {stageStatistics.Created}
                    Frissített: {stageStatistics.Updated}
                    Változatlan: {stageStatistics.Unchanged}
                """
            );
        }

        private ImportResult ImportMatch(
            SofaScoreEvent source,
            Dictionary<string, Team> teamsByExternalId,
            Dictionary<string, Competition> competitionsByExternalId,
            Dictionary<string, CompetitionEdition> editionsByExternalId,
            Dictionary<string, CompetitionStage> stagesByExternalId,
            Dictionary<string, Match> matchesByExternalId,
            ImportStatistics teamStatistics,
            ImportStatistics competitionStatistics,
            ImportStatistics editionStatistics,
            ImportStatistics stageStatistics,
            HashSet<string> countedTeamExternalIds,
            HashSet<string> countedCompetitionExternalIds,
            HashSet<string> countedEditionExternalIds,
            HashSet<string> countedStageExternalIds
        )
        {
            var (homeTeam, homeTeamResult) = GetOrCreateTeam(source.HomeTeam, teamsByExternalId);

            if (countedTeamExternalIds.Add(source.HomeTeam.Id.ToString()))
            {
                teamStatistics.Add(homeTeamResult);
            }

            var (awayTeam, awayTeamResult) = GetOrCreateTeam(source.AwayTeam, teamsByExternalId);

            if (countedTeamExternalIds.Add(source.AwayTeam.Id.ToString()))
            {
                teamStatistics.Add(awayTeamResult);
            }

            var (competition, competitionResult) = GetOrCreateCompetition(
                source,
                competitionsByExternalId
            );

            if (
                source.Tournament?.UniqueTournament is not null
                && countedCompetitionExternalIds.Add(
                    source.Tournament.UniqueTournament.Id.ToString()
                )
            )
            {
                competitionStatistics.Add(competitionResult);
            }

            var (edition, editionResult) = GetOrCreateCompetitionEdition(
                source,
                competition,
                editionsByExternalId
            );

            if (
                source.Season is not null
                && countedEditionExternalIds.Add(source.Season.Id.ToString())
            )
            {
                editionStatistics.Add(editionResult);
            }

            var (stage, stageResult) = GetOrCreateCompetitionStage(
                source,
                edition,
                stagesByExternalId
            );

            if (
                source.Season is not null
                && source.RoundInfo?.Round is not null
                && countedStageExternalIds.Add($"{source.Season.Id}_round_{source.RoundInfo.Round}")
            )
            {
                stageStatistics.Add(stageResult!.Value);
            }

            var date = DateTimeOffset.FromUnixTimeSeconds(source.StartTimestamp).UtcDateTime;

            var homeScore = source.HomeScore?.Current;
            var awayScore = source.AwayScore?.Current;

            var externalId = source.Id.ToString();

            if (!matchesByExternalId.TryGetValue(externalId, out var match))
            {
                match = new Match
                {
                    ExternalId = externalId,
                    DataSource = DataSource,
                    Date = date,
                    HomeTeam = homeTeam,
                    AwayTeam = awayTeam,
                    HomeScore = homeScore,
                    AwayScore = awayScore,
                    CompetitionEdition = edition,
                    CompetitionStage = stage,
                };

                dbContext.Matches.Add(match);

                matchesByExternalId.Add(externalId, match);

                return ImportResult.Created;
            }

            var hasChanges =
                match.Date != date
                || match.HomeTeamId != homeTeam.Id
                || match.AwayTeamId != awayTeam.Id
                || match.HomeScore != homeScore
                || match.AwayScore != awayScore
                || match.CompetitionEditionId != edition.Id
                || match.CompetitionStageId != stage?.Id;

            match.Date = date;
            match.HomeTeam = homeTeam;
            match.AwayTeam = awayTeam;
            match.HomeScore = homeScore;
            match.AwayScore = awayScore;
            match.CompetitionEdition = edition;
            match.CompetitionStage = stage;

            return hasChanges ? ImportResult.Updated : ImportResult.Unchanged;
        }

        private (Team Entity, ImportResult Result) GetOrCreateTeam(
            SofaScoreMatchTeam source,
            Dictionary<string, Team> teamsByExternalId
        )
        {
            var externalId = source.Id.ToString();

            if (teamsByExternalId.TryGetValue(externalId, out var team))
            {
                var hasChanges =
                    team.Name != source.Name
                    || team.Country != source.Country?.Name
                    || team.IsNationalTeam != source.National;

                team.Name = source.Name;
                team.Country = source.Country?.Name;
                team.IsNationalTeam = source.National;

                return (team, hasChanges ? ImportResult.Updated : ImportResult.Unchanged);
            }

            team = new Team
            {
                Name = source.Name,
                Country = source.Country?.Name,
                IsNationalTeam = source.National,
                ExternalId = externalId,
                DataSource = DataSource,
            };

            dbContext.Teams.Add(team);

            teamsByExternalId.Add(externalId, team);

            return (team, ImportResult.Created);
        }

        private (Competition Entity, ImportResult Result) GetOrCreateCompetition(
            SofaScoreEvent source,
            Dictionary<string, Competition> competitionsByExternalId
        )
        {
            var uniqueTournament = source.Tournament?.UniqueTournament;

            if (uniqueTournament is null)
            {
                throw new InvalidOperationException(
                    $"A mérkőzéshez nem tartozik torna: {source.Id}"
                );
            }

            var externalId = uniqueTournament.Id.ToString();

            if (competitionsByExternalId.TryGetValue(externalId, out var competition))
            {
                var hasChanges = competition.Name != uniqueTournament.Name;

                competition.Name = uniqueTournament.Name;

                return (competition, hasChanges ? ImportResult.Updated : ImportResult.Unchanged);
            }

            competition = new Competition
            {
                Name = uniqueTournament.Name,
                ExternalId = externalId,
                DataSource = DataSource,
            };

            dbContext.Competitions.Add(competition);

            competitionsByExternalId.Add(externalId, competition);

            return (competition, ImportResult.Created);
        }

        private (CompetitionEdition Entity, ImportResult Result) GetOrCreateCompetitionEdition(
            SofaScoreEvent source,
            Competition competition,
            Dictionary<string, CompetitionEdition> editionsByExternalId
        )
        {
            if (source.Season is null)
            {
                throw new InvalidOperationException(
                    $"A mérkőzéshez nem tartozik szezon: {source.Id}"
                );
            }

            var externalId = source.Season.Id.ToString();

            if (editionsByExternalId.TryGetValue(externalId, out var edition))
            {
                var hasChanges =
                    edition.Name != source.Season.Name
                    || edition.Year != source.Season.Year
                    || edition.CompetitionId != competition.Id;

                edition.Name = source.Season.Name;
                edition.Year = source.Season.Year;
                edition.Competition = competition;

                return (edition, hasChanges ? ImportResult.Updated : ImportResult.Unchanged);
            }

            edition = new CompetitionEdition
            {
                Name = source.Season.Name,
                Year = source.Season.Year,
                Competition = competition,
                ExternalId = externalId,
                DataSource = DataSource,
            };

            dbContext.CompetitionEditions.Add(edition);

            editionsByExternalId.Add(externalId, edition);

            return (edition, ImportResult.Created);
        }

        private (CompetitionStage? Entity, ImportResult? Result) GetOrCreateCompetitionStage(
            SofaScoreEvent source,
            CompetitionEdition edition,
            Dictionary<string, CompetitionStage> stagesByExternalId
        )
        {
            if (source.RoundInfo?.Round is null)
            {
                return (null, null);
            }

            var externalId = $"{source.Season!.Id}_round_{source.RoundInfo.Round}";

            var name = $"Round {source.RoundInfo.Round}";

            if (stagesByExternalId.TryGetValue(externalId, out var stage))
            {
                var hasChanges = stage.Name != name || stage.CompetitionEditionId != edition.Id;

                stage.Name = name;
                stage.CompetitionEdition = edition;

                return (stage, hasChanges ? ImportResult.Updated : ImportResult.Unchanged);
            }

            stage = new CompetitionStage
            {
                Name = name,
                CompetitionEdition = edition,
                ExternalId = externalId,
                DataSource = DataSource,
            };

            dbContext.CompetitionStages.Add(stage);

            stagesByExternalId.Add(externalId, stage);

            return (stage, ImportResult.Created);
        }
    }
}
