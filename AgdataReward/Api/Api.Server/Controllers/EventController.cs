using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Api.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EventController : ControllerBase
    {
        private readonly IEventService _eventService;

        public EventController(IEventService eventService)
        {
            _eventService = eventService;
        }

        // GET: api/event/{id}
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetEvent(Guid id)
        {
            var ev = await _eventService.GetEventByIdAsync(id);
            if (ev == null) return NotFound();
            return Ok(ev);
        }

        // POST: api/event
        [HttpPost]
        public async Task<IActionResult> CreateEvent([FromBody] EventDefinition dto)
        {
            var ev = await _eventService.CreateEventAsync(dto.Code, dto.Title);
            return CreatedAtAction(nameof(GetEvent), new { id = ev.Id }, ev);
        }

        // POST: api/event/{eventId}/rewardrule
        [HttpPost("{eventId:guid}/rewardrule")]
        public async Task<IActionResult> AddRewardRule(Guid eventId, [FromQuery] int rank, [FromQuery] Guid rewardPointsId)
        {
            await _eventService.AddRewardRuleAsync(eventId, rank, rewardPointsId);
            return Ok();
        }

        // POST: api/event/{instanceId}/assignwinner
        [HttpPost("{instanceId:guid}/assignwinner")]
        public async Task<IActionResult> AssignWinner(Guid instanceId, [FromQuery] Guid userId, [FromQuery] int rank)
        {
            await _eventService.AssignWinnerAsync(instanceId, userId, rank);
            return Ok();
        }
    }
}
