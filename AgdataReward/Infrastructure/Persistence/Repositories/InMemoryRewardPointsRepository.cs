using Application.Interfaces;
using Domain.Entities.Reward;

namespace Infrastructure.Persistence.Repositories;

public class InMemoryRewardPointsRepository : IRewardPointsRepository
{
    private readonly Dictionary<Guid, RewardPoints> _rewardPoints = new();

    public virtual Task<RewardPoints?> GetByIdAsync(Guid id)
    {
        _rewardPoints.TryGetValue(id, out var points);
        return Task.FromResult(points);
    }

    public Task AddAsync(RewardPoints rewardPoints)
    {
        _rewardPoints[rewardPoints.Id] = rewardPoints;
        return Task.CompletedTask;
    }
}
