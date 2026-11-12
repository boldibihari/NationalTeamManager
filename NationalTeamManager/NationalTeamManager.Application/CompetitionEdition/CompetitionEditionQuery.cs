namespace NationalTeamManager.Application.CompetitionEdition
{
    public record CompetitionEditionQuery(
        int? CompetitionId = null,
        string? Search = null,
        string? Year = null,
        string? SortBy = null,
        bool SortDescending = false,
        int Page = 1,
        int PageSize = 20
    );
}
