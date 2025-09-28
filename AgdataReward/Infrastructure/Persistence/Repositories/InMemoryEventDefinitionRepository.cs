using Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Persistence.Repositories
{
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
    }
    
}
