using NationalTeamManager.Application.Dtos.Player;

namespace NationalTeamManager.Application.Interfaces
{
    public interface IPlayerService : ICrudService<PlayerDto, CreatePlayerDto, UpdatePlayerDto> { }
}
