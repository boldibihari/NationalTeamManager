namespace NationalTeamManager.Application.Players
{
    public record PlayerQuery(
        string? Search = null,
        string? Position = null,
        int? TeamId = null,
        string? SortBy = null,
        bool SortDescending = false,
        int Page = 1,
        int PageSize = 20
    );
}
