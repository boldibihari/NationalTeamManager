using NationalTeamManager.Application.Common.Interfaces;

namespace NationalTeamManager.Application.Match.Dtos
{
    public record MatchDto(
        int Id,
        DateTime Date,
        int HomeTeamId,
        string HomeTeamName,
        int AwayTeamId,
        string AwayTeamName,
        int? HomeScore,
        int? AwayScore,
        string? Venue,
        int CompetitionEditionId,
        string CompetitionEditionName,
        int? CompetitionStageId,
        string? CompetitionStageName
    ) : IHasId;
}
