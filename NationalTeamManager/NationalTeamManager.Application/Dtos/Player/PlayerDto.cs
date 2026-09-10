using NationalTeamManager.Application.Interfaces;

namespace NationalTeamManager.Application.Dtos.Player
{
    public record PlayerDto(
        int Id,
        string Name,
        string Position,
        int? Height,
        DateOnly? DateOfBirth,
        string PreferredFoot,
        string? Nationality,
        string? TeamName,
        decimal? MarketValue,
        string? MarketValueCurrency
    ) : IHasId;
}
