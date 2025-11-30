using Application.Interfaces;
using Domain.Entities.Event;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public class EventDefinitionRepository(RewardDbContext context) : IEventDefinitionRepository
{
    private readonly RewardDbContext _context = context;

    public async Task<EventDefinition?> GetByIdAsync(Guid id)
    {
        // include related navigation properties
        return await _context.EventDefinitions
            .Include(e => e.Instances)
            .Include(e => e.RewardRules)
            .SingleOrDefaultAsync(e => e.Id == id);
    }

    public async Task AddAsync(EventDefinition definition)
    {
        _context.EventDefinitions.Add(definition);
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<EventDefinition>> ListAsync()
    {
        return await _context.EventDefinitions
            .AsNoTracking()
            .Include(e => e.RewardRules)
            .Include(e => e.Instances)
            .ToListAsync();
    }

    public async Task UpdateAsync(EventDefinition entity)
    {
        _context.EventDefinitions.Update(entity);
        await _context.SaveChangesAsync();
    }
}
