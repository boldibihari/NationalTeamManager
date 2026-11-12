using Mapster;
using NationalTeamManager.Application.Competition.Dtos;
using NationalTeamManager.Application.CompetitionEdition.Dtos;
using NationalTeamManager.Application.CompetitionStage.Dtos;
using NationalTeamManager.Domain.Entities;

namespace NationalTeamManager.Infrastructure.Mapping
{
    public static class CompetitionMappingConfiguration
    {
        public static void Register()
        {
            TypeAdapterConfig<Competition, CompetitionDto>.NewConfig();

            TypeAdapterConfig<CompetitionEdition, CompetitionEditionDto>
                .NewConfig()
                .Map(destination => destination.CompetitionName, source => source.Competition.Name);

            TypeAdapterConfig<CompetitionStage, CompetitionStageDto>
                .NewConfig()
                .Map(
                    destination => destination.CompetitionEditionName,
                    source => source.CompetitionEdition.Name
                )
                .Map(
                    destination => destination.CompetitionId,
                    source => source.CompetitionEdition.CompetitionId
                )
                .Map(
                    destination => destination.CompetitionName,
                    source => source.CompetitionEdition.Competition.Name
                );
        }
    }
}
