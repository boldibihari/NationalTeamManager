namespace NationalTeamManager.Application.Team
{
    public record TeamQuery(
        string? Search = null,
        string? Country = null,
        bool? IsNationalTeam = null,
        string? SortBy = null,
        bool SortDescending = false,
        int Page = 1,
        int PageSize = 20
    );
}
