using Application.Interfaces;
using Domain.Entities.Event;

namespace Infrastructure.Persistence.Repositories;

public class InMemoryEventInstanceRepository : IEventInstanceRepository
{
    private readonly List<EventInstance> _instances = new();

    public Task<EventInstance?> GetByIdAsync(Guid id)
        => Task.FromResult(_instances.FirstOrDefault(e => e.Id == id));

    public Task AddAsync(EventInstance instance)
    {
        _instances.Add(instance);
        return Task.CompletedTask;
    }
    public Task UpdateAsync(EventInstance instance)
    {
        return Task.CompletedTask;
    }


    public Task<IEnumerable<EventInstance>> ListAsync()
        => Task.FromResult<IEnumerable<EventInstance>>(_instances);

    public Task<IEnumerable<EventInstance>> GetByEventIdAsync(Guid eventId)
        => Task.FromResult<IEnumerable<EventInstance>>(_instances.Where(i => i.EventId == eventId).ToList());
}
