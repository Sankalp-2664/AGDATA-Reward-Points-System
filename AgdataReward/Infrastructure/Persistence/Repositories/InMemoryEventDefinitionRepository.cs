using Application.Interfaces;
using Domain.Entities.Event;

namespace Infrastructure.Persistence.Repositories;

public class InMemoryEventDefinitionRepository : IEventDefinitionRepository
{
    private readonly List<EventDefinition> _definitions = new();

    public Task<EventDefinition?> GetByIdAsync(Guid id)
        => Task.FromResult(_definitions.FirstOrDefault(d => d.Id == id));

    public Task AddAsync(EventDefinition definition)
    {
        _definitions.Add(definition);
        return Task.CompletedTask;
    }

    public Task<IEnumerable<EventDefinition>> ListAsync()
        => Task.FromResult<IEnumerable<EventDefinition>>(_definitions);

    public Task UpdateAsync(EventDefinition definition)
        {
        var index = _definitions.FindIndex(d => d.Id == definition.Id);
        if (index != -1)
        {
            _definitions[index] = definition;
        }
        return Task.CompletedTask;
    }
}

