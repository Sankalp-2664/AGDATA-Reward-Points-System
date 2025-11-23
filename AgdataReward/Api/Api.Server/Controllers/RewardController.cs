using Api.Server.DTOs.Reward;
using Application.Interfaces;
using Domain.Entities.Reward;
using Microsoft.AspNetCore.Mvc;

namespace Api.Server.Controllers;

[ApiController]
[Route("api/reward")]
public class RewardController : ControllerBase
{
    private readonly ITransactionService _transactionService;

    public RewardController(ITransactionService transactionService)
    {
        _transactionService = transactionService;
    }

    // GET: api/reward/user/{userId}
    [HttpGet("user/{userId:guid}")]
    public async Task<IActionResult> GetUserTransactions(Guid userId)
    {
        var transactions = await _transactionService.GetUserTransactionsAsync(userId);
        var dtos = transactions.Select(RewardTransactionDto.FromDomain);
        return Ok(dtos);
    }

    // POST: api/reward/points
    [HttpPost("points")]
    public async Task<IActionResult> CreateRewardPoints([FromBody] RewardPointsCreateDto dto)
    {
        var points = new RewardPoints(Guid.NewGuid(), dto.PointsValue);
        var created = await _transactionService.CreateRewardPointsAsync(points);
        return Ok(RewardPointsDto.FromDomain(created));
    }

    // GET: api/reward/points
    [HttpGet("points")]
    public async Task<IActionResult> GetRewardPoints()
    {
        var list = await _transactionService.ListRewardPointsAsync();
        var dtos = list.Select(RewardPointsDto.FromDomain);
        return Ok(dtos);
    }

    // GET: api/reward/points/{id}
    [HttpGet("points/{id:guid}")]
    public async Task<IActionResult> GetRewardPointsById(Guid id)
    {
        var points = await _transactionService.GetRewardPointsByIdAsync(id);
        if (points == null) return NotFound();
        return Ok(RewardPointsDto.FromDomain(points));
    }
}
