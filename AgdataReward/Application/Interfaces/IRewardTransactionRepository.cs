using Domain.Entities.Reward;

namespace Application.Interfaces;

public interface IRewardTransactionRepository
{
    Task<RewardTransaction?> GetByIdAsync(Guid id);
    Task AddAsync(RewardTransaction transaction);
    Task<IEnumerable<RewardTransaction>> GetByUserIdAsync(Guid userId);
    Task<bool> HasTransactionsForEventAsync(Guid eventId);
}
