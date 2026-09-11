using NationalTeamManager.Application.Common.Interfaces;
using NationalTeamManager.Application.Common.Models;
using NationalTeamManager.Application.Players.Dtos;

namespace NationalTeamManager.Application.Players.Interfaces
{
    public interface IPlayerService : ICrudService<PlayerDto, CreatePlayerDto, UpdatePlayerDto>
    {
        Task<PagedResult<PlayerDto>> SearchAsync(
            PlayerQuery query,
            CancellationToken cancellationToken = default
        );
    }
}
