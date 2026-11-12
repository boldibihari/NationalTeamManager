using NationalTeamManager.Application.Common.Interfaces;
using NationalTeamManager.Application.Common.Models;
using NationalTeamManager.Application.Match.Dtos;

namespace NationalTeamManager.Application.Match.Interfaces
{
    public interface IMatchService
    {
        Task<PagedResult<MatchDto>> SearchAsync(
            MatchQuery query,
            CancellationToken cancellationToken = default
        );

        Task<MatchDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    }
}
