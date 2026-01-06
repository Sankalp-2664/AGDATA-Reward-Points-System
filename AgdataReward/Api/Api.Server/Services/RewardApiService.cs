using Api.Server.DTOs.Reward;
using Application.Interfaces;
using AutoMapper;

namespace Api.Server.Services;

/// <summary>
/// API-facing reward service that coordinates domain services and mapping,
/// so controllers don't deal with domain entities or AutoMapper.
/// </summary>
public interface IRewardApiService
{
    Task<IEnumerable<RewardTransactionDto>> GetUserTransactionsAsync(
        Guid userId,
        CancellationToken cancellationToken = default);

    Task<RewardPointsDto> CreateRewardPointsAsync(
        RewardPointsCreateDto dto,
        CancellationToken cancellationToken = default);

    Task<IEnumerable<RewardPointsDto>> GetRewardPointsAsync(
        CancellationToken cancellationToken = default);

    Task<RewardPointsDto?> GetRewardPointsByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);
}

public class RewardApiService(
    ITransactionService transactionService,
    IMapper mapper) : IRewardApiService
{
    private readonly ITransactionService _transactionService = transactionService;
    private readonly IMapper _mapper = mapper;

    public async Task<IEnumerable<RewardTransactionDto>> GetUserTransactionsAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var transactions = await _transactionService.GetUserTransactionsAsync(userId);
        return _mapper.Map<IEnumerable<RewardTransactionDto>>(transactions);
    }

    public async Task<RewardPointsDto> CreateRewardPointsAsync(
        RewardPointsCreateDto dto,
        CancellationToken cancellationToken = default)
    {
        var created = await _transactionService.CreateRewardPointsAsync(dto.PointsValue);

        return _mapper.Map<RewardPointsDto>(created);
    }

    public async Task<IEnumerable<RewardPointsDto>> GetRewardPointsAsync(
        CancellationToken cancellationToken = default)
    {
        var list = await _transactionService.ListRewardPointsAsync();
        return _mapper.Map<IEnumerable<RewardPointsDto>>(list);
    }

    public async Task<RewardPointsDto?> GetRewardPointsByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var points = await _transactionService.GetRewardPointsByIdAsync(id);
        if (points is null)
            return null;

        return _mapper.Map<RewardPointsDto>(points);
    }
}
