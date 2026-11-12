namespace NationalTeamManager.Application.Competition
{
    public record CompetitionQuery(
        string? Search = null,
        string? Country = null,
        string? SortBy = null,
        bool SortDescending = false,
        int Page = 1,
        int PageSize = 20
    );
}
