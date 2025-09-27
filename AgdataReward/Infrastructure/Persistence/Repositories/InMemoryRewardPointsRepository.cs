using Application.Interfaces;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Persistence.Repositories
{
    public class InMemoryRewardPointsRepository : IRewardPointsRepository
    {
        private readonly Dictionary<Guid, RewardPoints> _rewardPoints = new();

        public Task<RewardPoints?> GetByIdAsync(Guid id)
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
}
