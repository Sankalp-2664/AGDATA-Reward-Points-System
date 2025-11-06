using Application.Interfaces;
using Domain.Entities.Redemption;
using Microsoft.EntityFrameworkCore;
using System;

namespace Infrastructure.Persistence.Repositories
{
    public class RedemptionRecordRepository : IRedemptionRecordRepository
    {
        private readonly RewardDbContext _context;

        public RedemptionRecordRepository(RewardDbContext context)
        {
            _context = context;
        }

        public async Task<RedemptionRecord?> GetByIdAsync(Guid id)
        {
            return await _context.RedemptionRecords
                .Include(r => r.User)
                .Include(r => r.Product)
                .FirstOrDefaultAsync(r => r.Id == id);
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
    }
}
