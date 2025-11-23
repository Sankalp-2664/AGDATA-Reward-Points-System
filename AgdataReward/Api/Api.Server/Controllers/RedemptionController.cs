using Api.Server.DTOs.Redemption;
using Application.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace Api.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RedemptionController : ControllerBase
{
    private readonly IRedemptionService _redemptionService;
    private readonly IMapper _mapper;

    public RedemptionController(IRedemptionService redemptionService, IMapper mapper)
    {
        _redemptionService = redemptionService;
        _mapper = mapper;
    }

    // Request a new redemption
    [HttpPost("request")]
    public async Task<IActionResult> RequestRedemption([FromBody] RedemptionRecordCreateDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var record = await _redemptionService.RequestRedemptionAsync(dto.UserId, dto.ProductId);
            var recordDto = _mapper.Map<RedemptionRecordDto>(record);
            return CreatedAtAction(nameof(GetRedemption), new { id = recordDto.Id }, recordDto);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    // Get redemption record by ID
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetRedemption(Guid id)
    {
        try
        {
            var record = await _redemptionService.GetRedemptionByIdAsync(id);
            if (record == null)
                return NotFound();

            var recordDto = _mapper.Map<RedemptionRecordDto>(record);
            return Ok(recordDto);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    // Approve redemption
    [HttpPost("{id:guid}/approve")]
    public async Task<IActionResult> ApproveRedemption(Guid id)
    {
        try
        {
            await _redemptionService.ApproveRedemptionAsync(id);
            return NoContent();
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    // Reject redemption
    [HttpPost("{id:guid}/reject")]
    public async Task<IActionResult> RejectRedemption(Guid id)
    {
        try
        {
            await _redemptionService.RejectRedemptionAsync(id);
            return NoContent();
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    // Complete redemption
    [HttpPost("{id:guid}/complete")]
    public async Task<IActionResult> CompleteRedemption(Guid id)
    {
        try
        {
            await _redemptionService.CompleteRedemptionAsync(id);
            return NoContent();
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}
