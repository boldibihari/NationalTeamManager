using Microsoft.AspNetCore.Mvc;
using NationalTeamManager.Application.Dtos.Player;
using NationalTeamManager.Application.Interfaces;

namespace NationalTeamManager.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PlayersController(IPlayerService playerService) : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<List<PlayerDto>>> GetAll(CancellationToken cancellationToken)
        {
            var players = await playerService.GetAllAsync(cancellationToken);

            return Ok(players);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<PlayerDto>> GetById(
            int id,
            CancellationToken cancellationToken
        )
        {
            var player = await playerService.GetByIdAsync(id, cancellationToken);

            if (player is null)
                return NotFound();

            return Ok(player);
        }

        [HttpPost]
        public async Task<ActionResult<PlayerDto>> Create(
            CreatePlayerDto dto,
            CancellationToken cancellationToken
        )
        {
            var player = await playerService.AddAsync(dto, cancellationToken);

            return Ok(player);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(
            int id,
            UpdatePlayerDto dto,
            CancellationToken cancellationToken
        )
        {
            var updated = await playerService.UpdateAsync(id, dto, cancellationToken);

            if (!updated)
                return NotFound();

            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
        {
            var deleted = await playerService.DeleteAsync(id, cancellationToken);

            if (!deleted)
                return NotFound();

            return NoContent();
        }
    }
}
