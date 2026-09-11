using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using NationalTeamManager.Application.Common.Interfaces;
using NationalTeamManager.Application.Common.Models;
using NationalTeamManager.Application.Players;
using NationalTeamManager.Application.Players.Dtos;
using NationalTeamManager.Application.Players.Interfaces;
using NationalTeamManager.Domain.Entities;
using NationalTeamManager.Domain.Enums;
using NationalTeamManager.Infrastructure.Services.Common;

namespace NationalTeamManager.Infrastructure.Services.Entities
{
    public class PlayerService(IMapper mapper, IEntityRepository<Player> repository)
        : CrudService<Player, PlayerDto, CreatePlayerDto, UpdatePlayerDto>(mapper, repository),
            IPlayerService
    {
        public override async Task<List<PlayerDto>> GetAllAsync(
            CancellationToken cancellationToken = default
        )
        {
            var players = await Repository.GetAllAsync(
                cancellationToken,
                x => x.Team!,
                x => x.MarketValues
            );

            return Mapper.Map<List<PlayerDto>>(players);
        }

        public override async Task<PlayerDto?> GetByIdAsync(
            int id,
            CancellationToken cancellationToken = default
        )
        {
            var player = await Repository.GetByIdAsync(
                id,
                cancellationToken,
                x => x.Team!,
                x => x.MarketValues
            );

            return player is null ? null : Mapper.Map<PlayerDto>(player);
        }

        public async Task<PagedResult<PlayerDto>> SearchAsync(
            PlayerQuery query,
            CancellationToken cancellationToken = default
        )
        {
            var players = Repository.Query(x => x.Team!, x => x.MarketValues);

            if (!string.IsNullOrWhiteSpace(query.Search))
            {
                players = players.Where(x => x.Name.Contains(query.Search));
            }

            if (
                !string.IsNullOrWhiteSpace(query.Position)
                && Enum.TryParse<PlayerPosition>(query.Position, true, out var position)
            )
            {
                players = players.Where(x => x.Position == position);
            }

            if (query.TeamId.HasValue)
            {
                players = players.Where(x => x.TeamId == query.TeamId);
            }

            players = query.SortBy?.ToLowerInvariant() switch
            {
                "name" => query.SortDescending
                    ? players.OrderByDescending(x => x.Name)
                    : players.OrderBy(x => x.Name),

                "marketvalue" => query.SortDescending
                    ? players.OrderByDescending(x =>
                        x.MarketValues.OrderByDescending(v => v.RecordedAt)
                            .Select(v => (decimal?)v.Value)
                            .FirstOrDefault()
                    )
                    : players.OrderBy(x =>
                        x.MarketValues.OrderByDescending(v => v.RecordedAt)
                            .Select(v => (decimal?)v.Value)
                            .FirstOrDefault()
                    ),

                "height" => query.SortDescending
                    ? players.OrderByDescending(x => x.Height)
                    : players.OrderBy(x => x.Height),

                _ => players.OrderBy(x => x.Name),
            };

            var totalCount = await players.CountAsync(cancellationToken);

            var skip = (query.Page - 1) * query.PageSize;

            var result = await players
                .Skip(skip)
                .Take(query.PageSize)
                .ToListAsync(cancellationToken);

            var items = Mapper.Map<List<PlayerDto>>(result);

            return new PagedResult<PlayerDto>(items, totalCount, query.Page, query.PageSize);
        }
    }
}
