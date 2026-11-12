using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using NationalTeamManager.Application.Common.Interfaces;
using NationalTeamManager.Application.Common.Models;
using NationalTeamManager.Application.Match;
using NationalTeamManager.Application.Match.Dtos;
using NationalTeamManager.Application.Match.Interfaces;
using NationalTeamManager.Domain.Entities;

namespace NationalTeamManager.Infrastructure.Services.Entities
{
    public class MatchService(IMapper mapper, IEntityRepository<Match> repository) : IMatchService
    {
        public async Task<PagedResult<MatchDto>> SearchAsync(
            MatchQuery query,
            CancellationToken cancellationToken = default
        )
        {
            var matches = repository.Query(
                x => x.HomeTeam,
                x => x.AwayTeam,
                x => x.CompetitionEdition,
                x => x.CompetitionStage!
            );

            if (query.From.HasValue)
            {
                matches = matches.Where(x => x.Date >= query.From.Value);
            }

            if (query.To.HasValue)
            {
                matches = matches.Where(x => x.Date <= query.To.Value);
            }

            if (query.TeamId.HasValue)
            {
                matches = matches.Where(x =>
                    x.HomeTeamId == query.TeamId || x.AwayTeamId == query.TeamId
                );
            }

            if (query.CompetitionEditionId.HasValue)
            {
                matches = matches.Where(x => x.CompetitionEditionId == query.CompetitionEditionId);
            }

            if (query.CompetitionStageId.HasValue)
            {
                matches = matches.Where(x => x.CompetitionStageId == query.CompetitionStageId);
            }

            matches = query.SortBy?.ToLowerInvariant() switch
            {
                "date" => query.SortDescending
                    ? matches.OrderByDescending(x => x.Date)
                    : matches.OrderBy(x => x.Date),

                _ => matches.OrderBy(x => x.Date),
            };

            var totalCount = await matches.CountAsync(cancellationToken);

            var skip = (query.Page - 1) * query.PageSize;

            var result = await matches
                .Skip(skip)
                .Take(query.PageSize)
                .ToListAsync(cancellationToken);

            var items = mapper.Map<List<MatchDto>>(result);

            return new PagedResult<MatchDto>(items, totalCount, query.Page, query.PageSize);
        }

        public async Task<MatchDto?> GetByIdAsync(
            int id,
            CancellationToken cancellationToken = default
        )
        {
            var match = await repository.GetByIdAsync(
                id,
                cancellationToken,
                x => x.HomeTeam,
                x => x.AwayTeam,
                x => x.CompetitionEdition,
                x => x.CompetitionStage!
            );

            return match is null ? null : mapper.Map<MatchDto>(match);
        }
    }
}
