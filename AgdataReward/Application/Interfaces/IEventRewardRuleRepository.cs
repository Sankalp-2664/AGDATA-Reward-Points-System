using Domain.Entities.Event;

namespace Application.Interfaces;

public interface IEventRewardRuleRepository
{
    Task<IEnumerable<EventRewardRule>> GetByEventIdAsync(Guid eventId);
    Task AddAsync(EventRewardRule rule);
    Task UpdateAsync(EventRewardRule rule);
}
