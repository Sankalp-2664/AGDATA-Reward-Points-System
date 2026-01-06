using Api.Server.DTOs.Redemption;
using Api.Server.Services;
using Application.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Server.Controllers;

/// <summary>
/// Manages redemption requests and approval workflow.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize] // All endpoints require authentication 
public class RedemptionController(IRedemptionApiService redemptionApiService) : ControllerBase
{
    private readonly IRedemptionApiService _redemptionApiService = redemptionApiService;

    /// <summary>
    /// Creates a redemption request for a product.
    /// </summary>
    /// <param name="dto">Redemption request payload.</param>
    /// <returns>The created redemption record.</returns>
    /// <remarks>
    /// Sample request:
    ///
    ///     POST /api/redemption/request
    ///     {
    ///        "userId": "b4fa4fb8-3837-43af-8f48-abb38f1a080f",
    ///        "productId": "7e8f734c-57da-4b19-8ad7-1c7f0b537e46"
    ///     }
    ///
    /// </remarks>
    /// <response code="201">Redemption request created successfully.</response>
    /// <response code="400">Invalid request or insufficient points.</response>
    [HttpPost("request")]
    [ProducesResponseType(typeof(RedemptionRecordDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> RequestRedemption([FromBody] RedemptionRecordCreateDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var result = await _redemptionApiService.RequestRedemptionAsync(dto);
            return CreatedAtAction(nameof(GetRedemption), new { id = result.Id }, result);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Retrieves a redemption record by ID.
    /// </summary>
    /// <param name="id">Redemption identifier.</param>
    /// <returns>A redemption record.</returns>
    /// <response code="200">Redemption found and returned.</response>
    /// <response code="404">Redemption not found.</response>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(RedemptionRecordDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetRedemption(Guid id)
    {
        var recordDto = await _redemptionApiService.GetRedemptionByIdAsync(id);

        if (recordDto == null)
            return NotFound();

        return Ok(recordDto);
    }

    /// <summary>
    /// Approves an existing redemption request.
    /// </summary>
    /// <param name="id">Redemption identifier.</param>
    /// <response code="204">Redemption successfully approved.</response>
    /// <response code="400">Invalid state transition.</response>
    /// <response code="403">Only admins can perform this action.</response>
    [HttpPost("{id:guid}/approve")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> ApproveRedemption(Guid id)
    {
        try
        {
            await _redemptionApiService.ApproveRedemptionAsync(id);
            return NoContent();
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Rejects a redemption request.
    /// </summary>
    /// <param name="id">Redemption identifier.</param>
    /// <response code="204">Redemption rejected successfully.</response>
    /// <response code="400">Invalid request or already processed.</response>
    /// <response code="403">Only admins can reject a redemption.</response>
    [HttpPost("{id:guid}/reject")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> RejectRedemption(Guid id)
    {
        try
        {
            await _redemptionApiService.RejectRedemptionAsync(id);
            return NoContent();
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Marks a redemption as completed and adjusts inventory and points.
    /// </summary>
    /// <param name="id">Redemption identifier.</param>
    /// <response code="204">Redemption completed successfully.</response>
    /// <response code="400">Invalid request or insufficient inventory.</response>
    /// <response code="403">Only admins can complete redemption.</response>
    [HttpPost("{id:guid}/complete")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> CompleteRedemption(Guid id)
    {
        try
        {
            await _redemptionApiService.CompleteRedemptionAsync(id);
            return NoContent();
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}
