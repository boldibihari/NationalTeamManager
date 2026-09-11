using NationalTeamManager.Domain.Enums;

namespace NationalTeamManager.Application.Players.Dtos
{
    public record UpdatePlayerDto(
        string Name,
        PlayerPosition Position,
        int? Height,
        DateOnly? DateOfBirth,
        PreferredFoot PreferredFoot,
        string? Nationality,
        int? TeamId
    );
}
