using Api.Server.DTOs.Event;
using Application.Interfaces;
using AutoMapper;
using Domain.Entities.Event;
using Microsoft.AspNetCore.Mvc;

namespace Api.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EventController : ControllerBase
{
    private readonly IEventService _eventService;
    private readonly IMapper _mapper;
    private readonly ILogger<EventController> _logger;

    public EventController(
        IEventService eventService,
        IMapper mapper,
        ILogger<EventController> logger)
    {
        _eventService = eventService;
        _mapper = mapper;
        _logger = logger;
    }

    // ============================
    // CREATE EVENT
    // POST: api/event
    // ============================
    [HttpPost]
    public async Task<IActionResult> CreateEvent([FromBody] EventDefinitionCreateDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var entity = _mapper.Map<EventDefinition>(dto);
            var created = await _eventService.CreateEventAsync(entity.Code, entity.Title);

            var resultDto = _mapper.Map<EventDefinitionDto>(created);
            return CreatedAtAction(nameof(GetById), new { id = resultDto.Id }, resultDto);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Duplicate event code");
            return Conflict(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating event");
            return StatusCode(500, new { message = "An error occurred while creating the event." });
        }
    }

    // ============================
    // GET ALL EVENTS
    // GET: api/event
    // ============================
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var events = await _eventService.ListEventsAsync();
        var dtos = _mapper.Map<IEnumerable<EventDefinitionDto>>(events);
        return Ok(dtos);
    }

    // ============================
    // GET EVENT BY ID
    // GET: api/event/{id}
    // ============================
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var entity = await _eventService.GetEventByIdAsync(id);
        if (entity == null)
            return NotFound();

        var dto = _mapper.Map<EventDefinitionDto>(entity);
        return Ok(dto);
    }

    // ============================
    // UPDATE EVENT
    // PUT: api/event/{id}
    // ============================
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] EventDefinitionUpdateDto dto)
    {
        if (id != dto.Id)
            return BadRequest("Event ID mismatch.");

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var existing = await _eventService.GetEventByIdAsync(id);
        if (existing == null)
            return NotFound();

        // Apply updates via AutoMapper
        _mapper.Map(dto, existing);
         await _eventService.UpdateEventAsync(existing);

        return Ok(_mapper.Map<EventDefinitionDto>(existing));
    }

    // ============================
    // ASSIGN WINNER
    // POST: api/event/assign-winner
    // ============================
    public record AssignWinnerRequest(Guid EventInstanceId, Guid UserId, int Rank);

    [HttpPost("assign-winner")]
    public async Task<IActionResult> AssignWinner([FromBody] AssignWinnerRequest request)
    {
        if (request.EventInstanceId == Guid.Empty || request.UserId == Guid.Empty)
            return BadRequest("Invalid identifiers.");

        try
        {
            await _eventService.AssignWinnerAsync(request.EventInstanceId, request.UserId, request.Rank);
            return Ok(new { message = "Winner assigned successfully." });
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Validation error assigning winner");
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error assigning winner");
            return StatusCode(500, new { message = "An error occurred while assigning the winner." });
        }
    }

    // ============================
    // ADD REWARD RULE
    // POST: api/event/{eventId}/reward-rule
    // ============================
    public record AddRewardRuleRequest(Guid RewardPointsId, int Rank);

    [HttpPost("{eventId:guid}/reward-rule")]
    public async Task<IActionResult> AddRewardRule(Guid eventId, [FromBody] AddRewardRuleRequest request)
    {
        if (eventId == Guid.Empty)
            return BadRequest("Event ID is required.");

        try
        {
            await _eventService.AddRewardRuleAsync(eventId, request.Rank, request.RewardPointsId);
            return Ok(new { message = "Reward rule added successfully." });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding reward rule");
            return StatusCode(500, new { message = "An error occurred while adding the reward rule." });
        }
    }
}
