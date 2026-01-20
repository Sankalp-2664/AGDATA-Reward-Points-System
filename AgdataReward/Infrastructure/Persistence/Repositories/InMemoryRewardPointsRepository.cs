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

    public Task<RewardPoints?> GetByValueAsync(int pointsValue)
    {
        var points = _rewardPoints.Values.FirstOrDefault(rp => rp.PointsValue == pointsValue);
        return Task.FromResult(points);
    }

    public Task AddAsync(RewardPoints rewardPoints)
    {
        _rewardPoints[rewardPoints.Id] = rewardPoints;
        return Task.CompletedTask;
    }

    public Task UpdateAsync(RewardPoints rewardPoints)
    {
        _rewardPoints[rewardPoints.Id] = rewardPoints;
        return Task.CompletedTask;
    }

    public async Task<IEnumerable<RewardPoints>> ListAsync()
    {
        return await Task.FromResult(_rewardPoints.Values.ToList());
    }
}
