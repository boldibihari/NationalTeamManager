using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using NationalTeamManager.Application.Common.Interfaces;
using NationalTeamManager.Application.Common.Models;
using NationalTeamManager.Application.CompetitionStage;
using NationalTeamManager.Application.CompetitionStage.Dtos;
using NationalTeamManager.Application.CompetitionStage.Interfaces;
using NationalTeamManager.Domain.Entities;

namespace NationalTeamManager.Infrastructure.Services.Entities
{
    public class CompetitionStageService(
        IMapper mapper,
        IEntityRepository<CompetitionStage> repository
    ) : ICompetitionStageService
    {
        public async Task<PagedResult<CompetitionStageDto>> SearchAsync(
            CompetitionStageQuery query,
            CancellationToken cancellationToken = default
        )
        {
            var stages = repository.Query(
                x => x.CompetitionEdition,
                x => x.CompetitionEdition.Competition
            );

            if (query.CompetitionEditionId.HasValue)
            {
                stages = stages.Where(x =>
                    x.CompetitionEditionId == query.CompetitionEditionId.Value
                );
            }

            if (!string.IsNullOrWhiteSpace(query.Search))
            {
                stages = stages.Where(x => x.Name.Contains(query.Search));
            }

            stages = query.SortBy?.ToLowerInvariant() switch
            {
                "name" => query.SortDescending
                    ? stages.OrderByDescending(x => x.Name)
                    : stages.OrderBy(x => x.Name),
                "competitioneditionname" => query.SortDescending
                    ? stages.OrderByDescending(x => x.CompetitionEdition.Name)
                    : stages.OrderBy(x => x.CompetitionEdition.Name),
                "competitionname" => query.SortDescending
                    ? stages.OrderByDescending(x => x.CompetitionEdition.Competition.Name)
                    : stages.OrderBy(x => x.CompetitionEdition.Competition.Name),
                _ => stages.OrderBy(x => x.Name),
            };

            var totalCount = await stages.CountAsync(cancellationToken);
            var result = await stages
                .Skip((query.Page - 1) * query.PageSize)
                .Take(query.PageSize)
                .ToListAsync(cancellationToken);

            return new PagedResult<CompetitionStageDto>(
                mapper.Map<List<CompetitionStageDto>>(result),
                totalCount,
                query.Page,
                query.PageSize
            );
        }

        public async Task<CompetitionStageDto?> GetByIdAsync(
            int id,
            CancellationToken cancellationToken = default
        )
        {
            var stage = await repository.GetByIdAsync(
                id,
                cancellationToken,
                x => x.CompetitionEdition,
                x => x.CompetitionEdition.Competition
            );

            return stage is null ? null : mapper.Map<CompetitionStageDto>(stage);
        }
    }
}
