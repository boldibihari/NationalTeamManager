using Microsoft.EntityFrameworkCore;
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

            foreach (var source in matches)
            {
                ImportMatch(
                    source,
                    teamsByExternalId,
                    competitionsByExternalId,
                    editionsByExternalId,
                    stagesByExternalId,
                    matchesByExternalId
                );
            }

            await dbContext.SaveChangesAsync(cancellationToken);

            Console.WriteLine($"Meccsimport kész: {matches.Count} mérkőzés feldolgozva.");
        }

        private void ImportMatch(
            SofaScoreEvent source,
            Dictionary<string, Team> teamsByExternalId,
            Dictionary<string, Competition> competitionsByExternalId,
            Dictionary<string, CompetitionEdition> editionsByExternalId,
            Dictionary<string, CompetitionStage> stagesByExternalId,
            Dictionary<string, Match> matchesByExternalId
        )
        {
            var homeTeam = GetOrCreateTeam(source.HomeTeam, teamsByExternalId);

            var awayTeam = GetOrCreateTeam(source.AwayTeam, teamsByExternalId);

            var competition = GetOrCreateCompetition(source, competitionsByExternalId);

            var edition = GetOrCreateCompetitionEdition(source, competition, editionsByExternalId);

            var stage = GetOrCreateCompetitionStage(source, edition, stagesByExternalId);

            var externalId = source.Id.ToString();

            if (!matchesByExternalId.TryGetValue(externalId, out var match))
            {
                match = new Match { ExternalId = externalId, DataSource = DataSource };

                dbContext.Matches.Add(match);

                matchesByExternalId.Add(externalId, match);
            }

            match.Date = DateTimeOffset.FromUnixTimeSeconds(source.StartTimestamp).UtcDateTime;

            match.HomeTeam = homeTeam;
            match.AwayTeam = awayTeam;

            match.HomeScore = source.HomeScore?.Current;
            match.AwayScore = source.AwayScore?.Current;

            match.CompetitionEdition = edition;
            match.CompetitionStage = stage;
        }

        private Team GetOrCreateTeam(
            SofaScoreMatchTeam source,
            Dictionary<string, Team> teamsByExternalId
        )
        {
            var externalId = source.Id.ToString();

            if (teamsByExternalId.TryGetValue(externalId, out var team))
            {
                team.Name = source.Name;
                team.Country = source.Country?.Name;
                team.IsNationalTeam = source.National;

                return team;
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

            return team;
        }

        private Competition GetOrCreateCompetition(
            SofaScoreEvent source,
            Dictionary<string, Competition> competitionsByExternalId
        )
        {
            if (source.Tournament?.UniqueTournament is null)
            {
                throw new InvalidOperationException(
                    $"A mérkőzéshez nem tartozik egyedi versenysorozat: {source.Id}"
                );
            }

            var tournament = source.Tournament.UniqueTournament;
            var externalId = tournament.Id.ToString();

            if (competitionsByExternalId.TryGetValue(externalId, out var competition))
            {
                competition.Name = tournament.Name;
                return competition;
            }

            competition = new Competition
            {
                Name = tournament.Name,
                ExternalId = externalId,
                DataSource = DataSource,
            };

            dbContext.Competitions.Add(competition);

            competitionsByExternalId.Add(externalId, competition);

            return competition;
        }

        private CompetitionEdition GetOrCreateCompetitionEdition(
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
                edition.Name = source.Season.Name;
                edition.Year = source.Season.Year;

                return edition;
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

            return edition;
        }

        private CompetitionStage? GetOrCreateCompetitionStage(
            SofaScoreEvent source,
            CompetitionEdition edition,
            Dictionary<string, CompetitionStage> stagesByExternalId
        )
        {
            if (source.RoundInfo?.Round is null)
            {
                return null;
            }

            var round = source.RoundInfo.Round.Value;

            var externalId = $"{source.Season?.Id}_round_{round}";

            if (stagesByExternalId.TryGetValue(externalId, out var stage))
            {
                stage.Name = $"Round {round}";

                return stage;
            }

            stage = new CompetitionStage
            {
                Name = $"Round {round}",
                CompetitionEdition = edition,
                ExternalId = externalId,
                DataSource = DataSource,
            };

            dbContext.CompetitionStages.Add(stage);

            stagesByExternalId.Add(externalId, stage);

            return stage;
        }
    }
}
