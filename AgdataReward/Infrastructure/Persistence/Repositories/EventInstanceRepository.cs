using Application.Interfaces;
using Domain.Entities.Event;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public class EventInstanceRepository(RewardDbContext context) : IEventInstanceRepository
{
    private readonly RewardDbContext _context = context;

    public async Task<EventInstance?> GetByIdAsync(Guid id)
    {
        return await _context.EventInstances
            .Include(i => i.Event)
            .Include(i => i.WinnerUser) // optional — to load the user
            .SingleOrDefaultAsync(i => i.Id == id);
    }

    public async Task AddAsync(EventInstance instance)
    {
        _context.EventInstances.Add(instance);
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<EventInstance>> ListAsync()
    {
        return await _context.EventInstances
            .AsNoTracking()
            .Include(i => i.Event)
            .Include(i => i.WinnerUser) // optional
            .ToListAsync();
    }
}
