using Api.Server.DTOs.Event;
using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Api.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EventsController : ControllerBase
{
    private readonly IEventService _eventService;

    public EventsController(IEventService eventService)
    {
        _eventService = eventService;
    }

    // GET: api/events/{id}
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetEvent(Guid id)
    {
        var ev = await _eventService.GetEventByIdAsync(id);
        if (ev == null)
            return NotFound();

        return Ok(EventInstanceD.FromDomain(ev));
    }

    // GET: api/events
    [HttpGet]
    public async Task<IActionResult> GetAllEvents()
    {
        var events = await _eventService.ListEventsAsync();
        var dtos = events.Select(EventDto.FromDomain);
        return Ok(dtos);
    }

    // POST: api/events
    [HttpPost]
    public async Task<IActionResult> CreateEvent([FromBody] CreateEventRequest request)
    {
        try
        {
            var ev = await _eventService.CreateEventAsync(request.Code, request.Title);
            var dto = EventDto.FromDomain(ev);

            return CreatedAtAction(nameof(GetEvent), new { id = dto.Id }, dto);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    // POST: api/events/{eventId}/rewardrule
    [HttpPost("{eventId:guid}/rewardrule")]
    public async Task<IActionResult> AddRewardRule(Guid eventId, [FromBody] AddRewardRuleRequest request)
    {
        await _eventService.AddRewardRuleAsync(eventId, request.Rank, request.RewardPointsId);
        return Ok();
    }

    // POST: api/events/{instanceId}/assignwinner
    [HttpPost("{instanceId:guid}/assignwinner")]
    public async Task<IActionResult> AssignWinner(Guid instanceId, [FromBody] AssignWinnerRequest request)
    {
        await _eventService.AssignWinnerAsync(instanceId, request.UserId, request.Rank);
        return Ok();
    }
}

// Request DTOs
public record CreateEventRequest(string Code, string Title);
public record AddRewardRuleRequest(int Rank, Guid RewardPointsId);
public record AssignWinnerRequest(Guid UserId, int Rank);
