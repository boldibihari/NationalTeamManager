using Microsoft.EntityFrameworkCore;
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

            foreach (var source in players)
            {
                ImportPlayer(
                    source,
                    playersByExternalId,
                    teamsByExternalId,
                    marketValuesByPlayerId,
                    recordedAt
                );
            }

            await dbContext.SaveChangesAsync(cancellationToken);

            Console.WriteLine($"Import kész: {players.Count} játékos feldolgozva.");
        }

        private void ImportPlayer(
            SofaScorePlayer source,
            Dictionary<string, Player> playersByExternalId,
            Dictionary<string, Team> teamsByExternalId,
            Dictionary<int, PlayerMarketValue> marketValuesByPlayerId,
            DateTime recordedAt
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

            var team = GetOrCreateTeam(source, teamsByExternalId);

            if (!playersByExternalId.TryGetValue(source.SofaScoreId, out var player))
            {
                player = new Player { ExternalId = source.SofaScoreId, DataSource = DataSource };

                dbContext.Players.Add(player);

                playersByExternalId.Add(source.SofaScoreId, player);
            }

            player.Name = source.Name;
            player.Position = MapPosition(source.Position);
            player.Height = source.Height;
            player.DateOfBirth = source.DateOfBirth.HasValue
                ? DateOnly.FromDateTime(source.DateOfBirth.Value.DateTime)
                : null;
            player.PreferredFoot = MapPreferredFoot(source.PreferredFoot);
            player.Nationality = source.Country?.Name;
            player.Team = team;

            UpsertMarketValue(player, source, marketValuesByPlayerId, recordedAt);
        }

        private Team GetOrCreateTeam(
            SofaScorePlayer source,
            Dictionary<string, Team> teamsByExternalId
        )
        {
            var externalId = source.Team!.Id.ToString();

            if (teamsByExternalId.TryGetValue(externalId, out var team))
            {
                team.Name = source.Team.Name;
                team.Country = source.Team.Country?.Name;

                return team;
            }

            team = new Team
            {
                Name = source.Team.Name,
                Country = source.Team.Country?.Name,
                IsNationalTeam = source.Team.Country is not null,
                ExternalId = externalId,
                DataSource = DataSource,
            };

            dbContext.Teams.Add(team);

            teamsByExternalId.Add(externalId, team);

            return team;
        }

        private void UpsertMarketValue(
            Player player,
            SofaScorePlayer source,
            Dictionary<int, PlayerMarketValue> marketValuesByPlayerId,
            DateTime recordedAt
        )
        {
            if (source.ProposedMarketValueRaw?.Value is null)
            {
                return;
            }

            if (
                player.Id != 0
                && marketValuesByPlayerId.TryGetValue(player.Id, out var existingMarketValue)
            )
            {
                existingMarketValue.Value = source.ProposedMarketValueRaw.Value.Value;

                existingMarketValue.Currency = source.ProposedMarketValueRaw.Currency ?? "EUR";

                return;
            }

            dbContext.PlayerMarketValues.Add(
                new PlayerMarketValue
                {
                    Player = player,
                    Value = source.ProposedMarketValueRaw.Value.Value,
                    Currency = source.ProposedMarketValueRaw.Currency ?? "EUR",
                    RecordedAt = recordedAt,
                }
            );
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
