using Domain.Entities.Event;

namespace Application.Interfaces;

public interface IEventDefinitionRepository
{
    Task<EventDefinition?> GetByIdAsync(Guid id);
    Task AddAsync(EventDefinition definition);
    Task<IEnumerable<EventDefinition>> ListAsync();
    Task UpdateAsync(EventDefinition definition);
}
