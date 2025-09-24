using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IEventRewardRuleRepository
    {
        Task<IEnumerable<EventRewardRule>> GetByEventIdAsync(Guid eventId);
        Task AddAsync(EventRewardRule rule);
        Task UpdateAsync(EventRewardRule rule);
    }
}
