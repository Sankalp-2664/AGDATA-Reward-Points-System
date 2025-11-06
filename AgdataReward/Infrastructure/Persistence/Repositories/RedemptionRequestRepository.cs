using Application.Interfaces;
using Domain.Entities.Redemption;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;
using System;

namespace Infrastructure.Persistence.Repositories
{
    public class RedemptionRequestRepository : IRedemptionRequestRepository
    {
        private readonly RewardDbContext _context;

        public RedemptionRequestRepository(RewardDbContext context)
        {
            _context = context;
        }

        public async Task<RedemptionRequest?> GetByIdAsync(Guid id)
        {
            return await _context.RedemptionRequests
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task UpdateAsync(RedemptionRequest request)
        {
            _context.RedemptionRequests.Update(request);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<RedemptionRequest>> GetPendingOrActiveByUserAndProductAsync(
            Guid userId,
            Guid productId,
            IEnumerable<RedemptionRecord> allRedemptionRecords)
        {
            var recordIds = allRedemptionRecords
                .Where(r => r.UserId == userId && r.ProductId == productId)
                .Select(r => r.Id)
                .ToList();

            return await _context.RedemptionRequests
                .Where(r => recordIds.Contains(r.RedemptionId) &&
                            (r.Status == RedemptionStatus.Pending || r.Status == RedemptionStatus.Approved))
                .ToListAsync();
        }
    }
}
