using Microsoft.AspNetCore.Mvc;
using NationalTeamManager.Application.Common.Models;
using NationalTeamManager.Application.Competition;
using NationalTeamManager.Application.Competition.Dtos;
using NationalTeamManager.Application.Competition.Interfaces;

namespace NationalTeamManager.Api.Controllers
{
    [ApiController]
    [Route("api/competitions")]
    public class CompetitionsController(ICompetitionService competitionService) : ControllerBase
    {
        [HttpGet]
        [ProducesResponseType(typeof(PagedResult<CompetitionDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<PagedResult<CompetitionDto>>> GetAll(
            [FromQuery] CompetitionQuery query,
            CancellationToken cancellationToken
        )
        {
            var competitions = await competitionService.SearchAsync(query, cancellationToken);

            return Ok(competitions);
        }

        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(CompetitionDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<CompetitionDto>> GetById(
            int id,
            CancellationToken cancellationToken
        )
        {
            var competition = await competitionService.GetByIdAsync(id, cancellationToken);

            if (competition is null)
                return NotFound();

            return Ok(competition);
        }
    }
}
