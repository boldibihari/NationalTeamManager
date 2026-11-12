namespace NationalTeamManager.Application.CompetitionStage
{
    public record CompetitionStageQuery(
        int? CompetitionEditionId = null,
        string? Search = null,
        string? SortBy = null,
        bool SortDescending = false,
        int Page = 1,
        int PageSize = 20
    );
}
