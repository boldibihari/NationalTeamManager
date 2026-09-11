using Mapster;
using NationalTeamManager.Application.Players.Dtos;
using NationalTeamManager.Domain.Entities;

namespace NationalTeamManager.Infrastructure.Mapping
{
    public static class MapsterConfiguration
    {
        public static void Register()
        {
            TypeAdapterConfig<Player, PlayerDto>
                .NewConfig()
                .Map(dest => dest.Position, src => src.Position.ToString())
                .Map(dest => dest.PreferredFoot, src => src.PreferredFoot.ToString())
                .Map(dest => dest.TeamName, src => src.Team != null ? src.Team.Name : null)
                .Map(
                    dest => dest.MarketValue,
                    src =>
                        src.MarketValues.OrderByDescending(x => x.RecordedAt)
                            .Select(x => (decimal?)x.Value)
                            .FirstOrDefault()
                )
                .Map(
                    dest => dest.MarketValueCurrency,
                    src =>
                        src.MarketValues.OrderByDescending(x => x.RecordedAt)
                            .Select(x => x.Currency)
                            .FirstOrDefault()
                );

            TypeAdapterConfig<CreatePlayerDto, Player>.NewConfig();

            TypeAdapterConfig<UpdatePlayerDto, Player>.NewConfig();
        }
    }
}
