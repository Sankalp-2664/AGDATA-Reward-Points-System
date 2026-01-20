using Application.Interfaces;
using Domain.Entities.Reward;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public class RewardPointsRepository(RewardDbContext context) : IRewardPointsRepository
{
    private readonly RewardDbContext _context = context;

    public async Task<RewardPoints?> GetByIdAsync(Guid id)
    {
        return await _context.RewardPoints
            .SingleOrDefaultAsync(rp => rp.Id == id);
    }

    public async Task<RewardPoints?> GetByValueAsync(int pointsValue)
    {
        return await _context.RewardPoints
            .FirstOrDefaultAsync(rp => rp.PointsValue == pointsValue);
    }

    public async Task AddAsync(RewardPoints rewardPoints)
    {
        await _context.RewardPoints.AddAsync(rewardPoints);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(RewardPoints rewardPoints)
    {
        _context.RewardPoints.Update(rewardPoints);
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<RewardPoints>> ListAsync()
    {
        return await _context.RewardPoints.ToListAsync();
    }
}
