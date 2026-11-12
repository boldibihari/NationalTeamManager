using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using NationalTeamManager.Application.Common.Interfaces;
using NationalTeamManager.Application.Common.Models;
using NationalTeamManager.Application.CompetitionEdition;
using NationalTeamManager.Application.CompetitionEdition.Dtos;
using NationalTeamManager.Application.CompetitionEdition.Interfaces;
using NationalTeamManager.Domain.Entities;

namespace NationalTeamManager.Infrastructure.Services.Entities
{
    public class CompetitionEditionService(
        IMapper mapper,
        IEntityRepository<CompetitionEdition> repository
    ) : ICompetitionEditionService
    {
        public async Task<PagedResult<CompetitionEditionDto>> SearchAsync(
            CompetitionEditionQuery query,
            CancellationToken cancellationToken = default
        )
        {
            var editions = repository.Query(x => x.Competition);

            if (query.CompetitionId.HasValue)
            {
                editions = editions.Where(x => x.CompetitionId == query.CompetitionId.Value);
            }

            if (!string.IsNullOrWhiteSpace(query.Search))
            {
                editions = editions.Where(x => x.Name.Contains(query.Search));
            }

            if (!string.IsNullOrWhiteSpace(query.Year))
            {
                editions = editions.Where(x => x.Year != null && x.Year.Contains(query.Year));
            }

            editions = query.SortBy?.ToLowerInvariant() switch
            {
                "name" => query.SortDescending
                    ? editions.OrderByDescending(x => x.Name)
                    : editions.OrderBy(x => x.Name),
                "year" => query.SortDescending
                    ? editions.OrderByDescending(x => x.Year)
                    : editions.OrderBy(x => x.Year),
                "competitionname" => query.SortDescending
                    ? editions.OrderByDescending(x => x.Competition.Name)
                    : editions.OrderBy(x => x.Competition.Name),
                _ => editions.OrderBy(x => x.Name),
            };

            var totalCount = await editions.CountAsync(cancellationToken);
            var result = await editions
                .Skip((query.Page - 1) * query.PageSize)
                .Take(query.PageSize)
                .ToListAsync(cancellationToken);

            return new PagedResult<CompetitionEditionDto>(
                mapper.Map<List<CompetitionEditionDto>>(result),
                totalCount,
                query.Page,
                query.PageSize
            );
        }

        public async Task<CompetitionEditionDto?> GetByIdAsync(
            int id,
            CancellationToken cancellationToken = default
        )
        {
            var edition = await repository.GetByIdAsync(id, cancellationToken, x => x.Competition);

            return edition is null ? null : mapper.Map<CompetitionEditionDto>(edition);
        }
    }
}
