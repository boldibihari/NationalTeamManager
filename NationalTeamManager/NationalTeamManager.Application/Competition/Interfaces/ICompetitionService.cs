using NationalTeamManager.Application.Common.Models;
using NationalTeamManager.Application.Competition.Dtos;

namespace NationalTeamManager.Application.Competition.Interfaces
{
    public interface ICompetitionService
    {
        Task<PagedResult<CompetitionDto>> SearchAsync(
            CompetitionQuery query,
            CancellationToken cancellationToken = default
        );
        Task<CompetitionDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    }
}
