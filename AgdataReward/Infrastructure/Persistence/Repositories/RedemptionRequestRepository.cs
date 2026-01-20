using Application.Interfaces;
using Domain.Entities.Redemption;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public class RedemptionRequestRepository(RewardDbContext context): IRedemptionRequestRepository
{
    private readonly RewardDbContext _context = context;

    public async Task<RedemptionRequest?> GetByIdAsync(Guid id)
    {
        return await _context.RedemptionRequests
            .SingleOrDefaultAsync(r => r.Id == id);
    }

    public async Task AddAsync(RedemptionRequest request)
    {
        await _context.RedemptionRequests.AddAsync(request);
        await _context.SaveChangesAsync();
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

    public async Task<IEnumerable<RedemptionRequest>> GetAllPendingAsync()
    {
        return await _context.RedemptionRequests
            .Where(r => r.Status == RedemptionStatus.Pending)
            .ToListAsync();
    }

    public async Task<IEnumerable<RedemptionRequest>> GetByRedemptionIdsAsync(IEnumerable<Guid> redemptionIds)
    {
        return await _context.RedemptionRequests
            .Where(r => redemptionIds.Contains(r.RedemptionId))
            .ToListAsync();
    }
}
