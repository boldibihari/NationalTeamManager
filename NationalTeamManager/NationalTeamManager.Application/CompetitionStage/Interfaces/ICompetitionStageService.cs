using NationalTeamManager.Application.Common.Models;
using NationalTeamManager.Application.CompetitionStage.Dtos;

namespace NationalTeamManager.Application.CompetitionStage.Interfaces
{
    public interface ICompetitionStageService
    {
        Task<PagedResult<CompetitionStageDto>> SearchAsync(
            CompetitionStageQuery query,
            CancellationToken cancellationToken = default
        );
        Task<CompetitionStageDto?> GetByIdAsync(
            int id,
            CancellationToken cancellationToken = default
        );
    }
}
