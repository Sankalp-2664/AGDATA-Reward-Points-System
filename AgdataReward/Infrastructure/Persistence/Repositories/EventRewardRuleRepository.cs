using Application.Interfaces;
using Domain.Entities.Event;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public class EventRewardRuleRepository : IEventRewardRuleRepository
{
    private readonly RewardDbContext _context;

    public EventRewardRuleRepository(RewardDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<EventRewardRule>> GetByEventIdAsync(Guid eventId)
    {
        return await _context.EventRewardRules
            .Where(r => r.EventId == eventId)
            .ToListAsync();
    }

    public async Task AddAsync(EventRewardRule rule)
    {
        await _context.EventRewardRules.AddAsync(rule);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(EventRewardRule rule)
    {
        // Optional: if rule is already tracked by EF, this is not strictly needed
        _context.EventRewardRules.Update(rule);
        await _context.SaveChangesAsync();
    }
}
