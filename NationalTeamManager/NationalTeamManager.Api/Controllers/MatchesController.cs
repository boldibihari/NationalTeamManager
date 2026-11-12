using Microsoft.AspNetCore.Mvc;
using NationalTeamManager.Application.Common.Models;
using NationalTeamManager.Application.Match;
using NationalTeamManager.Application.Match.Dtos;
using NationalTeamManager.Application.Match.Interfaces;

namespace NationalTeamManager.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MatchesController(IMatchService matchService) : ControllerBase
    {
        [HttpGet]
        [ProducesResponseType(typeof(PagedResult<MatchDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<PagedResult<MatchDto>>> GetAll(
            [FromQuery] MatchQuery query,
            CancellationToken cancellationToken
        )
        {
            var matches = await matchService.SearchAsync(query, cancellationToken);

            return Ok(matches);
        }

        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(MatchDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<MatchDto>> GetById(
            int id,
            CancellationToken cancellationToken
        )
        {
            var match = await matchService.GetByIdAsync(id, cancellationToken);

            if (match is null)
                return NotFound();

            return Ok(match);
        }
    }
}
