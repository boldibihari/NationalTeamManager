using Microsoft.AspNetCore.Mvc;
using NationalTeamManager.Application.Common.Models;
using NationalTeamManager.Application.CompetitionEdition;
using NationalTeamManager.Application.CompetitionEdition.Dtos;
using NationalTeamManager.Application.CompetitionEdition.Interfaces;

namespace NationalTeamManager.Api.Controllers
{
    [ApiController]
    [Route("api/competition-editions")]
    public class CompetitionEditionsController(ICompetitionEditionService competitionEditionService)
        : ControllerBase
    {
        [HttpGet]
        [ProducesResponseType(typeof(PagedResult<CompetitionEditionDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<PagedResult<CompetitionEditionDto>>> GetAll(
            [FromQuery] CompetitionEditionQuery query,
            CancellationToken cancellationToken
        )
        {
            var editions = await competitionEditionService.SearchAsync(query, cancellationToken);

            return Ok(editions);
        }

        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(CompetitionEditionDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<CompetitionEditionDto>> GetById(
            int id,
            CancellationToken cancellationToken
        )
        {
            var edition = await competitionEditionService.GetByIdAsync(id, cancellationToken);

            if (edition is null)
                return NotFound();

            return Ok(edition);
        }
    }
}
