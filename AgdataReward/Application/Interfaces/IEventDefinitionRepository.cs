using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IEventDefinitionRepository
    {
        Task<EventDefinition?> GetByIdAsync(Guid id);
        Task AddAsync(EventDefinition definition);
        Task<IEnumerable<EventDefinition>> ListAsync();
    }
}
