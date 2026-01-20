using Domain.Entities.Event;

namespace Application.Interfaces;

public interface IEventRewardRuleRepository
{
    Task<EventRewardRule?> GetByIdAsync(Guid id);
    Task<IEnumerable<EventRewardRule>> GetByEventIdAsync(Guid eventId);
    Task AddAsync(EventRewardRule rule);
    Task UpdateAsync(EventRewardRule rule);
    Task DeleteAsync(Guid id);
}
