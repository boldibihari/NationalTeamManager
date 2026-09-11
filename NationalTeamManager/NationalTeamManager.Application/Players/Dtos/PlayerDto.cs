using NationalTeamManager.Application.Common.Interfaces;

namespace NationalTeamManager.Application.Players.Dtos
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
