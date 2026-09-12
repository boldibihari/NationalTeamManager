using Microsoft.EntityFrameworkCore;
using NationalTeamManager.DataImporter.Models;
using NationalTeamManager.DataImporter.Providers.SofaScore;
using NationalTeamManager.DataImporter.Providers.SofaScore.Models;
using NationalTeamManager.Domain.Entities;
using NationalTeamManager.Domain.Enums;
using NationalTeamManager.Infrastructure.Persistence;

namespace NationalTeamManager.DataImporter.Importers
{
    public class PlayerImporter(
        ISofaScoreClient sofaScoreClient,
        NationalTeamManagerDbContext dbContext
    )
    {
        private const string DataSource = "SofaScore";

        public async Task ImportAsync(int teamId, CancellationToken cancellationToken = default)
        {
            var squad = await sofaScoreClient.GetSquadAsync(teamId, cancellationToken);

            var players = squad.Players.Select(x => x.Player).ToList();

            var playerExternalIds = players
                .Select(x => x.SofaScoreId)
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Distinct()
                .ToList();

            var teamExternalIds = players
                .Where(x => x.Team is not null)
                .Select(x => x.Team!.Id.ToString())
                .Distinct()
                .ToList();

            var existingPlayers = await dbContext
                .Players.Where(x =>
                    x.DataSource == DataSource
                    && x.ExternalId != null
                    && playerExternalIds.Contains(x.ExternalId)
                )
                .ToListAsync(cancellationToken);

            var existingTeams = await dbContext
                .Teams.Where(x =>
                    x.DataSource == DataSource
                    && x.ExternalId != null
                    && teamExternalIds.Contains(x.ExternalId)
                )
                .ToListAsync(cancellationToken);

            var existingPlayerIds = existingPlayers.Select(x => x.Id).ToList();

            var recordedAt = DateTime.UtcNow.Date;

            var existingMarketValues = await dbContext
                .PlayerMarketValues.Where(x =>
                    existingPlayerIds.Contains(x.PlayerId) && x.RecordedAt == recordedAt
                )
                .ToListAsync(cancellationToken);

            var playersByExternalId = existingPlayers.ToDictionary(
                x => x.ExternalId!,
                StringComparer.Ordinal
            );

            var teamsByExternalId = existingTeams.ToDictionary(
                x => x.ExternalId!,
                StringComparer.Ordinal
            );

            var marketValuesByPlayerId = existingMarketValues.ToDictionary(x => x.PlayerId);

            var playerStatistics = new ImportStatistics();
            var teamStatistics = new ImportStatistics();
            var marketValueStatistics = new ImportStatistics();

            var countedTeamExternalIds = new HashSet<string>(StringComparer.Ordinal);

            foreach (var source in players)
            {
                var result = ImportPlayer(
                    source,
                    playersByExternalId,
                    teamsByExternalId,
                    marketValuesByPlayerId,
                    recordedAt,
                    teamStatistics,
                    marketValueStatistics,
                    countedTeamExternalIds
                );

                playerStatistics.Add(result);
            }

            await dbContext.SaveChangesAsync(cancellationToken);

            Console.WriteLine(
                $"""
                Játékosimport kész:

                  Játékosok:
                    Összesen: {players.Count}
                    Új: {playerStatistics.Created}
                    Frissített: {playerStatistics.Updated}
                    Változatlan: {playerStatistics.Unchanged}

                  Csapatok:
                    Új: {teamStatistics.Created}
                    Frissített: {teamStatistics.Updated}
                    Változatlan: {teamStatistics.Unchanged}

                  Piaci értékek:
                    Új: {marketValueStatistics.Created}
                    Frissített: {marketValueStatistics.Updated}
                    Változatlan: {marketValueStatistics.Unchanged}
                """
            );
        }

        private ImportResult ImportPlayer(
            SofaScorePlayer source,
            Dictionary<string, Player> playersByExternalId,
            Dictionary<string, Team> teamsByExternalId,
            Dictionary<int, PlayerMarketValue> marketValuesByPlayerId,
            DateTime recordedAt,
            ImportStatistics teamStatistics,
            ImportStatistics marketValueStatistics,
            HashSet<string> countedTeamExternalIds
        )
        {
            if (string.IsNullOrWhiteSpace(source.SofaScoreId))
            {
                throw new InvalidOperationException(
                    $"A játékosnak nincs SofaScore azonosítója: {source.Name}"
                );
            }

            if (source.Team is null)
            {
                throw new InvalidOperationException(
                    $"A játékoshoz nem tartozik csapat: {source.Name}"
                );
            }

            var (team, teamResult) = GetOrCreateTeam(source, teamsByExternalId);

            var teamExternalId = source.Team.Id.ToString();

            if (countedTeamExternalIds.Add(teamExternalId))
            {
                teamStatistics.Add(teamResult);
            }

            var position = MapPosition(source.Position);
            var preferredFoot = MapPreferredFoot(source.PreferredFoot);

            DateOnly? dateOfBirth = source.DateOfBirth.HasValue
                ? DateOnly.FromDateTime(source.DateOfBirth.Value.DateTime)
                : null;

            if (!playersByExternalId.TryGetValue(source.SofaScoreId, out var player))
            {
                player = new Player
                {
                    ExternalId = source.SofaScoreId,
                    DataSource = DataSource,
                    Name = source.Name,
                    Position = position,
                    Height = source.Height,
                    DateOfBirth = dateOfBirth,
                    PreferredFoot = preferredFoot,
                    Nationality = source.Country?.Name,
                    Team = team,
                };

                dbContext.Players.Add(player);

                playersByExternalId.Add(source.SofaScoreId, player);

                var marketValueResult = UpsertMarketValue(
                    player,
                    source,
                    marketValuesByPlayerId,
                    recordedAt
                );

                marketValueStatistics.Add(marketValueResult);

                return ImportResult.Created;
            }

            var hasChanges =
                player.Name != source.Name
                || player.Position != position
                || player.Height != source.Height
                || player.DateOfBirth != dateOfBirth
                || player.PreferredFoot != preferredFoot
                || player.Nationality != source.Country?.Name
                || player.TeamId != team.Id;

            player.Name = source.Name;
            player.Position = position;
            player.Height = source.Height;
            player.DateOfBirth = dateOfBirth;
            player.PreferredFoot = preferredFoot;
            player.Nationality = source.Country?.Name;
            player.Team = team;

            var marketValueResultForExistingPlayer = UpsertMarketValue(
                player,
                source,
                marketValuesByPlayerId,
                recordedAt
            );

            marketValueStatistics.Add(marketValueResultForExistingPlayer);

            return hasChanges ? ImportResult.Updated : ImportResult.Unchanged;
        }

        private (Team Entity, ImportResult Result) GetOrCreateTeam(
            SofaScorePlayer source,
            Dictionary<string, Team> teamsByExternalId
        )
        {
            var externalId = source.Team!.Id.ToString();

            if (teamsByExternalId.TryGetValue(externalId, out var team))
            {
                var hasChanges =
                    team.Name != source.Team.Name
                    || team.Country != source.Team.Country?.Name
                    || team.IsNationalTeam != (source.Team.Country is not null);

                team.Name = source.Team.Name;
                team.Country = source.Team.Country?.Name;
                team.IsNationalTeam = false;

                return (team, hasChanges ? ImportResult.Updated : ImportResult.Unchanged);
            }

            team = new Team
            {
                Name = source.Team.Name,
                Country = source.Team.Country?.Name,
                IsNationalTeam = false,
                ExternalId = externalId,
                DataSource = DataSource,
            };

            dbContext.Teams.Add(team);

            teamsByExternalId.Add(externalId, team);

            return (team, ImportResult.Created);
        }

        private ImportResult UpsertMarketValue(
            Player player,
            SofaScorePlayer source,
            Dictionary<int, PlayerMarketValue> marketValuesByPlayerId,
            DateTime recordedAt
        )
        {
            if (source.ProposedMarketValueRaw?.Value is null)
            {
                return ImportResult.Unchanged;
            }

            var value = source.ProposedMarketValueRaw.Value.Value;
            var currency = source.ProposedMarketValueRaw.Currency ?? "EUR";

            if (
                player.Id != 0
                && marketValuesByPlayerId.TryGetValue(player.Id, out var existingMarketValue)
            )
            {
                var hasChanges =
                    existingMarketValue.Value != value || existingMarketValue.Currency != currency;

                existingMarketValue.Value = value;
                existingMarketValue.Currency = currency;

                return hasChanges ? ImportResult.Updated : ImportResult.Unchanged;
            }

            dbContext.PlayerMarketValues.Add(
                new PlayerMarketValue
                {
                    Player = player,
                    Value = value,
                    Currency = currency,
                    RecordedAt = recordedAt,
                }
            );

            return ImportResult.Created;
        }

        private static PlayerPosition MapPosition(string? position)
        {
            return position switch
            {
                "G" => PlayerPosition.Goalkeeper,
                "D" => PlayerPosition.Defender,
                "M" => PlayerPosition.Midfielder,
                "F" => PlayerPosition.Forward,
                _ => throw new InvalidOperationException(
                    $"Ismeretlen SofaScore pozíció: {position}"
                ),
            };
        }

        private static PreferredFoot MapPreferredFoot(string? preferredFoot)
        {
            return preferredFoot switch
            {
                "Right" => PreferredFoot.Right,
                "Left" => PreferredFoot.Left,
                "Both" => PreferredFoot.Both,
                _ => throw new InvalidOperationException(
                    $"Ismeretlen preferált láb: {preferredFoot}"
                ),
            };
        }
    }
}
