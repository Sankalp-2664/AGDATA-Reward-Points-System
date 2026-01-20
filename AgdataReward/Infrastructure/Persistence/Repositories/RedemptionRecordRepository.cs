using Application.Interfaces;
using Domain.Entities.Redemption;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public class RedemptionRecordRepository(RewardDbContext context) : IRedemptionRecordRepository
{
    private readonly RewardDbContext _context = context;

    public async Task<RedemptionRecord?> GetByIdAsync(Guid id)
    {
        return await _context.RedemptionRecords
            .Include(r => r.User)
            .Include(r => r.Product)
            .SingleOrDefaultAsync(r => r.Id == id);
    }

    public async Task AddAsync(RedemptionRecord record)
    {
        await _context.RedemptionRecords.AddAsync(record);
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<RedemptionRecord>> GetAllAsync()
    {
        return await _context.RedemptionRecords
            .Include(r => r.User)
            .Include(r => r.Product)
            .ToListAsync();
    }

    public async Task<IEnumerable<RedemptionRecord>> GetByUserIdAsync(Guid userId)
    {
        return await _context.RedemptionRecords
            .Include(r => r.User)
            .Include(r => r.Product)
            .Where(r => r.UserId == userId)
            .ToListAsync();
    }
}
