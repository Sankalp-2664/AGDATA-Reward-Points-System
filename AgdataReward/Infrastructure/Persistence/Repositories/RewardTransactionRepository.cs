using Application.Interfaces;
using Domain.Entities.Reward;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public class RewardTransactionRepository(RewardDbContext context) : IRewardTransactionRepository
{
    private readonly RewardDbContext _context = context;

    public async Task<RewardTransaction?> GetByIdAsync(Guid id)
    {
        return await _context.RewardTransactions
            .Include(t => t.UserAccount)
            .Include(t => t.EventInstance)
            .Include(t => t.RedemptionRequest)
            .SingleOrDefaultAsync(t => t.Id == id);
    }

    public async Task AddAsync(RewardTransaction transaction)
    {
        await _context.RewardTransactions.AddAsync(transaction);
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<RewardTransaction>> GetByUserIdAsync(Guid userId)
    {
        return await _context.RewardTransactions
            .Where(t => t.UserId == userId)
            .Include(t => t.EventInstance)
            .Include(t => t.RedemptionRequest)
            .ToListAsync();
    }

    public async Task<bool> HasTransactionsForEventAsync(Guid eventId)
    {
        // Since EventId references EventInstance (not EventDefinition), 
        // we check the Notes field which contains the event ID
        var eventIdString = eventId.ToString();
        return await _context.RewardTransactions
            .AnyAsync(t => t.Notes.Contains(eventIdString));
    }
}
