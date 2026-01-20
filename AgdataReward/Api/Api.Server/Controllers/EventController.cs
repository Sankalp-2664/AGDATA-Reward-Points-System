using Api.Server.DTOs.Event;
using Api.Server.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Api.Server.Controllers;

/// <summary>
/// Manages event definitions, reward rules and winner assignments
/// in the reward system.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[AllowAnonymous] 
public class EventController(
    IEventApiService eventApiService,
    ILogger<EventController> logger) : ControllerBase
{
    private readonly IEventApiService _eventApiService = eventApiService;
    private readonly ILogger<EventController> _logger = logger;
    /// <summary>
    /// Creates a new event definition.
    /// </summary>
    /// <param name="dto">The event definition payload.</param>
    /// <returns>The newly created event definition.</returns>
    /// <remarks>
    /// Sample request:
    ///
    ///     POST /api/event
    ///     {
    ///       "code": "HACK2025",
    ///       "title": "Hackathon 2025"
    ///     }
    ///
    /// </remarks>
    /// <response code="201">Returns the newly created event definition.</response>
    /// <response code="400">If the input payload is invalid.</response>
    /// <response code="401">If the caller is not authenticated.</response>
    /// <response code="403">If the caller is not an admin.</response>
    /// <response code="409">If an event with the same code already exists.</response>
    /// <response code="500">If an unexpected error occurs.</response>
    [HttpPost]
    // [Authorize(Roles = "Admin")] // Temporarily disabled for testing
    [ProducesResponseType(typeof(EventDefinitionDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CreateEvent([FromBody] EventDefinitionCreateDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var resultDto = await _eventApiService.CreateEventAsync(dto);
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

    /// <summary>
    /// Retrieves all event definitions.
    /// </summary>
    /// <returns>List of event definitions.</returns>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<EventDefinitionDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetAll()
    {
        var dtos = await _eventApiService.GetAllAsync();
        return Ok(dtos);
    }

    /// <summary>
    /// Retrieves all event definitions with reward points details.
    /// </summary>
    /// <returns>List of events with reward rule information including points values.</returns>
    [HttpGet("with-rewards")]
    [ProducesResponseType(typeof(IEnumerable<EventWithRewardsDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetAllWithRewards()
    {
        // Get current user ID if authenticated
        Guid? currentUserId = null;
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!string.IsNullOrEmpty(userIdClaim) && Guid.TryParse(userIdClaim, out var userId))
        {
            currentUserId = userId;
        }

        var dtos = await _eventApiService.GetAllWithRewardsAsync(currentUserId);
        return Ok(dtos);
    }

    /// <summary>
    /// Retrieves an event definition by its identifier.
    /// </summary>
    /// <param name="id">Event identifier.</param>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(EventDefinitionDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id)
    {
        var dto = await _eventApiService.GetByIdAsync(id);
        if (dto is null)
            return NotFound();

        return Ok(dto);
    }

    /// <summary>
    /// Updates an existing event definition.
    /// </summary>
    [HttpPut("{id:guid}")]
    // [Authorize(Roles = "Admin")] // Temporarily disabled for testing
    [ProducesResponseType(typeof(EventDefinitionDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(Guid id, [FromBody] EventDefinitionUpdateDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var updated = await _eventApiService.UpdateAsync(id, dto);
            if (updated is null)
                return NotFound();

            return Ok(updated);
        }
        catch (ArgumentException ex) when (ex.Message.Contains("mismatch", StringComparison.OrdinalIgnoreCase))
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    public record AssignWinnerRequest(Guid EventInstanceId, Guid UserId, int Rank);

    [HttpPost("assign-winner")]
    // [Authorize(Roles = "Admin")] // Temporarily disabled for testing
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> AssignWinner([FromBody] AssignWinnerRequest request)
    {
        if (request.EventInstanceId == Guid.Empty || request.UserId == Guid.Empty)
            return BadRequest("Invalid identifiers.");

        try
        {
            await _eventApiService.AssignWinnerAsync(request.EventInstanceId, request.UserId, request.Rank);
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

    public record UpdateStatusRequest(string Status);

    /// <summary>
    /// Updates the status of an event.
    /// </summary>
    [HttpPut("{id:guid}/status")]
    [ProducesResponseType(typeof(EventDefinitionDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] UpdateStatusRequest request)
    {
        try
        {
            var updated = await _eventApiService.UpdateEventStatusAsync(id, request.Status);
            return Ok(updated);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Error updating event status");
            return BadRequest(new { message = ex.Message });
        }
    }

    public record CompleteEventRequest(Guid? FirstPlaceUserId, Guid? SecondPlaceUserId, Guid? ThirdPlaceUserId);

    /// <summary>
    /// Completes an event and assigns winners, awarding them points.
    /// </summary>
    [HttpPost("{id:guid}/complete")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CompleteWithWinners(Guid id, [FromBody] CompleteEventRequest request)
    {
        try
        {
            _logger.LogInformation("🏆 Completing event {EventId} with winners: 1st={First}, 2nd={Second}, 3rd={Third}",
                id, request.FirstPlaceUserId, request.SecondPlaceUserId, request.ThirdPlaceUserId);
            
            await _eventApiService.CompleteEventWithWinnersAsync(id, request.FirstPlaceUserId, request.SecondPlaceUserId, request.ThirdPlaceUserId);
            return Ok(new { message = "Event completed and winners assigned successfully." });
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Error completing event");
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error completing event with winners");
            return StatusCode(500, new { message = "An error occurred while completing the event." });
        }
    }

    public record AddRewardRuleRequest(int Rank, Guid RewardPointsId);

    [HttpPost("{eventId:guid}/reward-rule")]
    // [Authorize(Roles = "Admin")] // Temporarily disabled for testing
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> AddRewardRule(Guid eventId, [FromBody] AddRewardRuleRequest request)
    {
        _logger.LogInformation("📬 AddRewardRule called - EventId: {EventId}, Rank: {Rank}, RewardPointsId: {RewardPointsId}", 
            eventId, request.Rank, request.RewardPointsId);
        
        if (eventId == Guid.Empty)
        {
            _logger.LogWarning("❌ Event ID is empty");
            return BadRequest("Event ID is required.");
        }

        try
        {
            await _eventApiService.AddRewardRuleAsync(eventId, request.Rank, request.RewardPointsId);
            _logger.LogInformation("✅ Reward rule added successfully for EventId: {EventId}, Rank: {Rank}", eventId, request.Rank);
            return Ok(new { message = "Reward rule added successfully." });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error adding reward rule for EventId: {EventId}", eventId);
            return StatusCode(500, new { message = "An error occurred while adding the reward rule." });
        }
    }

    public record UpdateRewardRuleRequest(Guid RewardPointsId);

    [HttpPut("reward-rule/{ruleId:guid}")]
    // [Authorize(Roles = "Admin")] // Temporarily disabled for testing
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateRewardRule(Guid ruleId, [FromBody] UpdateRewardRuleRequest request)
    {
        try
        {
            await _eventApiService.UpdateRewardRuleAsync(ruleId, request.RewardPointsId);
            return Ok(new { message = "Reward rule updated successfully." });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating reward rule");
            return BadRequest(new { message = ex.Message });
        }
    }

    [AllowAnonymous] // Temporarily allow for testing
    [HttpPost("{eventInstanceId}/participate")]
    public async Task<IActionResult> Participate(Guid eventInstanceId)
    {
        var userId = Guid.Parse(
            User.FindFirstValue(ClaimTypes.NameIdentifier)!
        );

        await _eventApiService.ParticipateAsync(eventInstanceId, userId);

        return Ok(new { message = "Participation successful." });
    }

}
