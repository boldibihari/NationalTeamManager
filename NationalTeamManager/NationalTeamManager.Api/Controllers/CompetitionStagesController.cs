using Microsoft.AspNetCore.Mvc;
using NationalTeamManager.Application.Common.Models;
using NationalTeamManager.Application.CompetitionStage;
using NationalTeamManager.Application.CompetitionStage.Dtos;
using NationalTeamManager.Application.CompetitionStage.Interfaces;

namespace NationalTeamManager.Api.Controllers
{
    [ApiController]
    [Route("api/competition-stages")]
    public class CompetitionStagesController(ICompetitionStageService competitionStageService)
        : ControllerBase
    {
        [HttpGet]
        [ProducesResponseType(typeof(PagedResult<CompetitionStageDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<PagedResult<CompetitionStageDto>>> GetAll(
            [FromQuery] CompetitionStageQuery query,
            CancellationToken cancellationToken
        )
        {
            var stages = await competitionStageService.SearchAsync(query, cancellationToken);

            return Ok(stages);
        }

        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(CompetitionStageDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<CompetitionStageDto>> GetById(
            int id,
            CancellationToken cancellationToken
        )
        {
            var stage = await competitionStageService.GetByIdAsync(id, cancellationToken);

            if (stage is null)
                return NotFound();

            return Ok(stage);
        }
    }
}
