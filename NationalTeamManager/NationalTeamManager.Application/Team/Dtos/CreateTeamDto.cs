namespace NationalTeamManager.Application.Team.Dtos
{
    public record CreateTeamDto(string Name, string? Country, bool IsNationalTeam);
}
