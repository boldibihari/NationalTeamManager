using NationalTeamManager.Application.Common.Interfaces;

namespace NationalTeamManager.Application.Competition.Dtos
{
    public record CompetitionDto(int Id, string Name, string? Country) : IHasId;
}
