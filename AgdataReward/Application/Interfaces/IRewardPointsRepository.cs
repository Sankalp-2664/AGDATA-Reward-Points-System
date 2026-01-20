using Domain.Entities.Reward;

namespace Application.Interfaces;

public interface IRewardPointsRepository
{
    Task<RewardPoints?> GetByIdAsync(Guid id);
    Task<RewardPoints?> GetByValueAsync(int pointsValue);
    Task AddAsync(RewardPoints rewardPoints);
    Task UpdateAsync(RewardPoints rewardPoints);
    Task<IEnumerable<RewardPoints>> ListAsync();
}
