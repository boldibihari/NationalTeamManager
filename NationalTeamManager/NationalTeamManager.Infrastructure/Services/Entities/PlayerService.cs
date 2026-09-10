using MapsterMapper;
using NationalTeamManager.Application.Dtos.Player;
using NationalTeamManager.Application.Interfaces;
using NationalTeamManager.Domain.Entities;
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
    }
}
