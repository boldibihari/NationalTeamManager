using NationalTeamManager.Application.Common.Interfaces;

namespace NationalTeamManager.Application.Team.Dtos
{
    public record TeamDto(int Id, string Name, string? Country, bool IsNationalTeam) : IHasId;
}
