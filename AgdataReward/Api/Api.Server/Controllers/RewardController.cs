using Api.Server.DTOs.Reward;
using Application.Interfaces;
using Api.Server.Services;
using AutoMapper;
using Domain.Entities.Reward;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Server.Controllers;

/// <summary>
/// Handles reward points management and user reward transactions.
/// </summary>
[ApiController]
[Route("api/reward")]
[Authorize] // All routes require authentication unless overridden
public class RewardController(IRewardApiService rewardApiService) : ControllerBase
{
    private readonly IRewardApiService _rewardApiService = rewardApiService;

    /// <summary>
    /// Retrieves all reward transactions for a specific user.
    /// </summary>
    /// <param name="userId">The unique identifier of the user.</param>
    /// <returns>A list of reward transactions.</returns>
    /// <remarks>
    /// Sample request:
    ///
    ///     GET /api/reward/user/{userId}
    ///
    /// Sample response:
    ///
    ///     200 OK
    ///     [
    ///       {
    ///         "id": "d55306b6-89d4-4c1e-af03-8a39dd271eb4",
    ///         "pointsDelta": 50,
    ///         "type": "Credit",
    ///         "createdAt": "2025-11-29T10:00:00Z",
    ///         "notes": "Event reward"
    ///       }
    ///     ]
    /// </remarks>
    /// <response code="200">Returns the transaction list successfully.</response>
    /// <response code="401">Caller is not authenticated.</response>
    [HttpGet("user/{userId:guid}")]
    [ProducesResponseType(typeof(IEnumerable<RewardTransactionDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetUserTransactions(Guid userId)
    {
        var dtos = await _rewardApiService.GetUserTransactionsAsync(userId);
        return Ok(dtos);
    }

    /// <summary>
    /// Creates a new reward point configuration.
    /// </summary>
    /// <param name="dto">Reward point value definition.</param>
    /// <returns>The newly created reward point entry.</returns>
    /// <remarks>
    /// Sample request:
    ///
    ///     POST /api/reward/points
    ///     {
    ///         "pointsValue": 200
    ///     }
    ///
    /// Sample response:
    ///
    ///     201 Created
    ///     {
    ///       "id": "bb2a1e24-0607-4e03-bb88-5e2e5940802f",
    ///       "pointsValue": 200
    ///     }
    /// </remarks>
    /// <response code="201">Returns the created reward point entry.</response>
    /// <response code="400">If the input is invalid.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">Only admins can create reward configurations.</response>
    [HttpPost("points")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(RewardPointsDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> CreateRewardPoints([FromBody] RewardPointsCreateDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var createdDto = await _rewardApiService.CreateRewardPointsAsync(dto);

        return CreatedAtAction(nameof(GetRewardPointsById), new { id = createdDto.Id }, createdDto);
    }

    /// <summary>
    /// Lists all available reward point configurations.
    /// </summary>
    /// <returns>A list of reward point configurations.</returns>
    /// <remarks>
    /// Sample request:
    ///
    ///     GET /api/reward/points
    ///
    /// Sample response:
    ///
    ///     200 OK
    ///     [
    ///       { "id": "a1", "pointsValue": 50 },
    ///       { "id": "a2", "pointsValue": 100 }
    ///     ]
    /// </remarks>
    /// <response code="200">Returns available reward options.</response>
    /// <response code="401">User is not authenticated.</response>
    [HttpGet("points")]
    [ProducesResponseType(typeof(IEnumerable<RewardPointsDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetRewardPoints()
    {
        var dtos = await _rewardApiService.GetRewardPointsAsync();
        return Ok(dtos);
    }

    /// <summary>
    /// Retrieves a reward point definition by its identifier.
    /// </summary>
    /// <param name="id">The reward point ID.</param>
    /// <returns>A reward point configuration.</returns>
    /// <remarks>
    /// Sample request:
    ///
    ///     GET /api/reward/points/{id}
    ///
    /// Sample response:
    ///
    ///     200 OK
    ///     {
    ///       "id": "bb2a1e24-0607-4e03-bb88-5e2e5940802f",
    ///       "pointsValue": 200
    ///     }
    /// </remarks>
    /// <response code="200">Successfully returned the reward configuration.</response>
    /// <response code="404">Reward point entry was not found.</response>
    [HttpGet("points/{id:guid}")]
    [ProducesResponseType(typeof(RewardPointsDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetRewardPointsById(Guid id)
    {
        var dto = await _rewardApiService.GetRewardPointsByIdAsync(id);
        if (dto is null)
            return NotFound();

        return Ok(dto);
    }
}
