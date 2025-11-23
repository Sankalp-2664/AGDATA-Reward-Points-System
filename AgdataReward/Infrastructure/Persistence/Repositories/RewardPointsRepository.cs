using Application.Interfaces;
using Domain.Entities.Reward;
using Microsoft.EntityFrameworkCore;
using System;

namespace Infrastructure.Persistence.Repositories
{
    public class RewardPointsRepository : IRewardPointsRepository
    {
        private readonly RewardDbContext _context;

        public RewardPointsRepository(RewardDbContext context)
        {
            _context = context;
        }

        public async Task<RewardPoints?> GetByIdAsync(Guid id)
        {
            return await _context.RewardPoints
                .FirstOrDefaultAsync(rp => rp.Id == id);
        }

        public async Task AddAsync(RewardPoints rewardPoints)
        {
            await _context.RewardPoints.AddAsync(rewardPoints);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<RewardPoints>> ListAsync()
        {
            return await _context.RewardPoints.ToListAsync();
        }

    }
}
