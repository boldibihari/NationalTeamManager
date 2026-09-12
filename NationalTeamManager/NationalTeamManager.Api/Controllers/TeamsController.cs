using Microsoft.AspNetCore.Mvc;
using NationalTeamManager.Application.Common.Models;
using NationalTeamManager.Application.Team;
using NationalTeamManager.Application.Team.Dtos;
using NationalTeamManager.Application.Team.Interfaces;

namespace NationalTeamManager.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TeamsController(ITeamService teamService) : ControllerBase
    {
        [HttpGet]
        [ProducesResponseType(typeof(PagedResult<TeamDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<PagedResult<TeamDto>>> GetAll(
            [FromQuery] TeamQuery query,
            CancellationToken cancellationToken
        )
        {
            var teams = await teamService.SearchAsync(query, cancellationToken);

            return Ok(teams);
        }

        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(TeamDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<TeamDto>> GetById(
            int id,
            CancellationToken cancellationToken
        )
        {
            var team = await teamService.GetByIdAsync(id, cancellationToken);

            if (team is null)
                return NotFound();

            return Ok(team);
        }

        [HttpPost]
        [ProducesResponseType(typeof(TeamDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<TeamDto>> Create(
            CreateTeamDto dto,
            CancellationToken cancellationToken
        )
        {
            var team = await teamService.AddAsync(dto, cancellationToken);

            return CreatedAtAction(nameof(GetById), new { id = team.Id }, team);
        }

        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(
            int id,
            UpdateTeamDto dto,
            CancellationToken cancellationToken
        )
        {
            var updated = await teamService.UpdateAsync(id, dto, cancellationToken);

            if (!updated)
                return NotFound();

            return NoContent();
        }

        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
        {
            var deleted = await teamService.DeleteAsync(id, cancellationToken);

            if (!deleted)
                return NotFound();

            return NoContent();
        }
    }
}
