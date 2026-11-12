using NationalTeamManager.Application.Common.Interfaces;

namespace NationalTeamManager.Application.CompetitionStage.Dtos
{
    public record CompetitionStageDto(
        int Id,
        string Name,
        int CompetitionEditionId,
        string CompetitionEditionName,
        int CompetitionId,
        string CompetitionName
    ) : IHasId;
}
