using NationalTeamManager.Domain.Enums;

namespace NationalTeamManager.Application.Dtos.Player
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
