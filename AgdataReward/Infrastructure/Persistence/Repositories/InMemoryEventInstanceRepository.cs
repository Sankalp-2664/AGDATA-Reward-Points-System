using Application.Interfaces;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Persistence.Repositories
{
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

        public Task<IEnumerable<EventInstance>> ListAsync()
            => Task.FromResult<IEnumerable<EventInstance>>(_instances);
    }
}
