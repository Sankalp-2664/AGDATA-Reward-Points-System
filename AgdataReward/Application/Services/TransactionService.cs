using Application.Interfaces;
using Domain.Entities.Reward;

namespace Application.Services;

public class TransactionService(
    IRewardTransactionRepository transactionRepo,
    IRewardPointsRepository rewardPointsRepo) : ITransactionService
{
    private readonly IRewardTransactionRepository _transactionRepo = transactionRepo;
    private readonly IRewardPointsRepository _rewardPointsRepo = rewardPointsRepo;

    public async Task<RewardPoints> CreateRewardPointsAsync(int pointsValue)
    {
        var rewardPoints = new RewardPoints(Guid.NewGuid(), pointsValue);
        await _rewardPointsRepo.AddAsync(rewardPoints);
        return rewardPoints;
    }

    public async Task<RewardPoints?> GetRewardPointsByIdAsync(Guid id)
    {
        return await _rewardPointsRepo.GetByIdAsync(id);
    }

    public async Task<IEnumerable<RewardPoints>> ListRewardPointsAsync()
    {
        return await _rewardPointsRepo.ListAsync();
    }

    public async Task<IEnumerable<RewardTransaction>> GetUserTransactionsAsync(Guid userId)
    {
        return await _transactionRepo.GetByUserIdAsync(userId);
    }
}