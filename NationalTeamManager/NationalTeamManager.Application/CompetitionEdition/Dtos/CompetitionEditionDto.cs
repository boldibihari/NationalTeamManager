using NationalTeamManager.Application.Common.Interfaces;

namespace NationalTeamManager.Application.CompetitionEdition.Dtos
{
    public record CompetitionEditionDto(
        int Id,
        string Name,
        string? Year,
        int CompetitionId,
        string CompetitionName
    ) : IHasId;
}
