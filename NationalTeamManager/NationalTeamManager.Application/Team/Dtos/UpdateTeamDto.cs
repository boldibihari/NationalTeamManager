namespace NationalTeamManager.Application.Team.Dtos
{
    public record UpdateTeamDto(string Name, string? Country, bool IsNationalTeam);
}
