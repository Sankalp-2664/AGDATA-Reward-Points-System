using Application.Interfaces;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Persistence.Repositories
{
    public class InMemoryEventRewardRuleRepository : IEventRewardRuleRepository
    {
        private readonly List<EventRewardRule> _rules = new();

        public Task<IEnumerable<EventRewardRule>> GetByEventIdAsync(Guid eventId)
            => Task.FromResult<IEnumerable<EventRewardRule>>(_rules.Where(r => r.EventId == eventId));

        public Task AddAsync(EventRewardRule rule)
        {
            _rules.Add(rule);
            return Task.CompletedTask;
        }

        public Task UpdateAsync(EventRewardRule rule)
        {
            var index = _rules.FindIndex(r => r.Id == rule.Id);
            if (index >= 0)
                _rules[index] = rule;
            return Task.CompletedTask;
        }
    }
}
