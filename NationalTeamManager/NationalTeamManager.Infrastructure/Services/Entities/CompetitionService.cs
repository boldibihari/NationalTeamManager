using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using NationalTeamManager.Application.Common.Interfaces;
using NationalTeamManager.Application.Common.Models;
using NationalTeamManager.Application.Competition;
using NationalTeamManager.Application.Competition.Dtos;
using NationalTeamManager.Application.Competition.Interfaces;
using NationalTeamManager.Domain.Entities;

namespace NationalTeamManager.Infrastructure.Services.Entities
{
    public class CompetitionService(IMapper mapper, IEntityRepository<Competition> repository)
        : ICompetitionService
    {
        public async Task<PagedResult<CompetitionDto>> SearchAsync(
            CompetitionQuery query,
            CancellationToken cancellationToken = default
        )
        {
            var competitions = repository.Query();

            if (!string.IsNullOrWhiteSpace(query.Search))
            {
                competitions = competitions.Where(x => x.Name.Contains(query.Search));
            }

            if (!string.IsNullOrWhiteSpace(query.Country))
            {
                competitions = competitions.Where(x =>
                    x.Country != null && x.Country.Contains(query.Country)
                );
            }

            competitions = query.SortBy?.ToLowerInvariant() switch
            {
                "name" => query.SortDescending
                    ? competitions.OrderByDescending(x => x.Name)
                    : competitions.OrderBy(x => x.Name),
                "country" => query.SortDescending
                    ? competitions.OrderByDescending(x => x.Country)
                    : competitions.OrderBy(x => x.Country),
                _ => competitions.OrderBy(x => x.Name),
            };

            var totalCount = await competitions.CountAsync(cancellationToken);
            var result = await competitions
                .Skip((query.Page - 1) * query.PageSize)
                .Take(query.PageSize)
                .ToListAsync(cancellationToken);

            return new PagedResult<CompetitionDto>(
                mapper.Map<List<CompetitionDto>>(result),
                totalCount,
                query.Page,
                query.PageSize
            );
        }

        public async Task<CompetitionDto?> GetByIdAsync(
            int id,
            CancellationToken cancellationToken = default
        )
        {
            var competition = await repository.GetByIdAsync(id, cancellationToken);

            return competition is null ? null : mapper.Map<CompetitionDto>(competition);
        }
    }
}
