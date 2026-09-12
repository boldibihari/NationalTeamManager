using NationalTeamManager.Application.Common.Interfaces;
using NationalTeamManager.Application.Common.Models;
using NationalTeamManager.Application.Team.Dtos;

namespace NationalTeamManager.Application.Team.Interfaces
{
    public interface ITeamService : ICrudService<TeamDto, CreateTeamDto, UpdateTeamDto>
    {
        Task<PagedResult<TeamDto>> SearchAsync(
            TeamQuery query,
            CancellationToken cancellationToken = default
        );
    }
}
