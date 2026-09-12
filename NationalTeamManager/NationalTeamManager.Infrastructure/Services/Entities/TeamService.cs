using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using NationalTeamManager.Application.Common.Interfaces;
using NationalTeamManager.Application.Common.Models;
using NationalTeamManager.Application.Team;
using NationalTeamManager.Application.Team.Dtos;
using NationalTeamManager.Application.Team.Interfaces;
using NationalTeamManager.Domain.Entities;
using NationalTeamManager.Infrastructure.Services.Common;

namespace NationalTeamManager.Infrastructure.Services.Entities
{
    public class TeamService(IMapper mapper, IEntityRepository<Team> repository)
        : CrudService<Team, TeamDto, CreateTeamDto, UpdateTeamDto>(mapper, repository),
            ITeamService
    {
        public async Task<PagedResult<TeamDto>> SearchAsync(
            TeamQuery query,
            CancellationToken cancellationToken = default
        )
        {
            var teams = Repository.Query();

            if (!string.IsNullOrWhiteSpace(query.Search))
            {
                teams = teams.Where(x => x.Name.Contains(query.Search));
            }

            if (!string.IsNullOrWhiteSpace(query.Country))
            {
                teams = teams.Where(x => x.Country != null && x.Country.Contains(query.Country));
            }

            if (query.IsNationalTeam.HasValue)
            {
                teams = teams.Where(x => x.IsNationalTeam == query.IsNationalTeam);
            }

            teams = query.SortBy?.ToLowerInvariant() switch
            {
                "name" => query.SortDescending
                    ? teams.OrderByDescending(x => x.Name)
                    : teams.OrderBy(x => x.Name),

                "country" => query.SortDescending
                    ? teams.OrderByDescending(x => x.Country)
                    : teams.OrderBy(x => x.Country),

                _ => teams.OrderBy(x => x.Name),
            };

            var totalCount = await teams.CountAsync(cancellationToken);

            var skip = (query.Page - 1) * query.PageSize;

            var result = await teams.Skip(skip).Take(query.PageSize).ToListAsync(cancellationToken);

            var items = Mapper.Map<List<TeamDto>>(result);

            return new PagedResult<TeamDto>(items, totalCount, query.Page, query.PageSize);
        }
    }
}
