using NationalTeamManager.Application.Common.Models;
using NationalTeamManager.Application.CompetitionEdition.Dtos;

namespace NationalTeamManager.Application.CompetitionEdition.Interfaces
{
    public interface ICompetitionEditionService
    {
        Task<PagedResult<CompetitionEditionDto>> SearchAsync(
            CompetitionEditionQuery query,
            CancellationToken cancellationToken = default
        );
        Task<CompetitionEditionDto?> GetByIdAsync(
            int id,
            CancellationToken cancellationToken = default
        );
    }
}
