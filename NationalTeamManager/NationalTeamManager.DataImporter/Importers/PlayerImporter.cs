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
        public async Task ImportAsync(int teamId, CancellationToken cancellationToken = default)
        {
            var squad = await sofaScoreClient.GetSquadAsync(teamId, cancellationToken);

            foreach (var item in squad.Players)
            {
                await ImportPlayerAsync(item.Player, cancellationToken);
            }

            await dbContext.SaveChangesAsync(cancellationToken);
        }

        private async Task ImportPlayerAsync(
            SofaScorePlayer source,
            CancellationToken cancellationToken
        )
        {
            var club = await GetOrCreateClubAsync(source, cancellationToken);

            var player = await dbContext.Players.FirstOrDefaultAsync(
                x => x.ExternalId == source.SofaScoreId && x.DataSource == "SofaScore",
                cancellationToken
            );

            if (player is null)
            {
                player = new Player
                {
                    Name = source.Name,
                    Position = MapPosition(source.Position),
                    Height = source.Height,
                    DateOfBirth = source.DateOfBirth.HasValue
                        ? DateOnly.FromDateTime(source.DateOfBirth.Value.DateTime)
                        : null,
                    PreferredFoot = MapPreferredFoot(source.PreferredFoot),
                    Nationality = source.Country?.Name,
                    ExternalId = source.SofaScoreId,
                    DataSource = "SofaScore",
                    Club = club,
                };

                dbContext.Players.Add(player);
            }
            else
            {
                player.Name = source.Name;
                player.Position = MapPosition(source.Position);
                player.Height = source.Height;
                player.DateOfBirth = source.DateOfBirth.HasValue
                    ? DateOnly.FromDateTime(source.DateOfBirth.Value.DateTime)
                    : null;
                player.PreferredFoot = MapPreferredFoot(source.PreferredFoot);
                player.Nationality = source.Country?.Name;
                player.Club = club;
            }

            await UpsertMarketValueAsync(player, source, cancellationToken);
        }

        private async Task<Club> GetOrCreateClubAsync(
            SofaScorePlayer source,
            CancellationToken cancellationToken
        )
        {
            if (source.Team is null)
            {
                throw new InvalidOperationException(
                    $"A játékoshoz nem tartozik klub: {source.Name}"
                );
            }

            var externalId = source.Team.Id.ToString();

            // Már ebben a DbContext-ben létrehozott klub keresése
            var club = dbContext.Clubs.Local.FirstOrDefault(x =>
                x.ExternalId == externalId && x.DataSource == "SofaScore"
            );

            if (club is not null)
            {
                return club;
            }

            // Adatbázisban már létező klub keresése
            club = await dbContext.Clubs.FirstOrDefaultAsync(
                x => x.ExternalId == externalId && x.DataSource == "SofaScore",
                cancellationToken
            );

            if (club is not null)
            {
                return club;
            }

            // Új klub
            club = new Club
            {
                Name = source.Team.Name,
                Country = source.Team.Country?.Name,
                ExternalId = externalId,
                DataSource = "SofaScore",
            };

            dbContext.Clubs.Add(club);

            return club;
        }

        private async Task UpsertMarketValueAsync(
            Player player,
            SofaScorePlayer source,
            CancellationToken cancellationToken
        )
        {
            if (source.ProposedMarketValueRaw?.Value is null)
            {
                return;
            }

            var recordedAt = DateTime.UtcNow.Date;

            var marketValue = await dbContext.PlayerMarketValues.FirstOrDefaultAsync(
                x => x.Player == player && x.RecordedAt == recordedAt,
                cancellationToken
            );

            if (marketValue is null)
            {
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
            else
            {
                marketValue.Value = source.ProposedMarketValueRaw.Value.Value;

                marketValue.Currency = source.ProposedMarketValueRaw.Currency ?? "EUR";
            }
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
