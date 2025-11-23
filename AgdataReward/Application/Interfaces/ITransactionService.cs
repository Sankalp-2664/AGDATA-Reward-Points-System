using Domain.Entities.Reward;

namespace Application.Interfaces;

public interface ITransactionService
{
    Task<IEnumerable<RewardTransaction>> GetUserTransactionsAsync(Guid userId);
    Task<RewardPoints> CreateRewardPointsAsync(RewardPoints rewardPoints);
    Task<RewardPoints?> GetRewardPointsByIdAsync(Guid id);
    Task<IEnumerable<RewardPoints>> ListRewardPointsAsync();
}
