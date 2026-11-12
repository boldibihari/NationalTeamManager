namespace NationalTeamManager.Application.Match
{
    public record MatchQuery(
        DateTime? From = null,
        DateTime? To = null,
        int? TeamId = null,
        int? CompetitionEditionId = null,
        int? CompetitionStageId = null,
        string? SortBy = null,
        bool SortDescending = false,
        int Page = 1,
        int PageSize = 20
    );
}
