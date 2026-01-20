using Domain.Entities.Event;

namespace Application.Interfaces;

public interface IEventInstanceRepository
{
    Task<EventInstance?> GetByIdAsync(Guid id);
    Task AddAsync(EventInstance instance);
    Task<IEnumerable<EventInstance>> ListAsync();
    Task UpdateAsync(EventInstance instance);
    Task<IEnumerable<EventInstance>> GetByEventIdAsync(Guid eventId);
}
