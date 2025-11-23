using Domain.Entities.Reward;

namespace Application.Interfaces;

public interface IRewardPointsRepository
{
    Task<RewardPoints?> GetByIdAsync(Guid id);
    Task AddAsync(RewardPoints rewardPoints);
    Task<IEnumerable<RewardPoints>> ListAsync();

}
