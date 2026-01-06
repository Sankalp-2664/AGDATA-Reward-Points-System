using Domain.Entities.Event;

namespace Application.Interfaces;

public interface IEventService
{
    Task<EventDefinition> CreateEventAsync(string code, string title);
    Task<EventDefinition?> GetEventByIdAsync(Guid id);
    Task<IEnumerable<EventDefinition>> ListEventsAsync();
    Task<EventDefinition> UpdateEventAsync(EventDefinition definition);
    Task AddRewardRuleAsync(Guid eventId, int rank, Guid rewardPointsId);
    Task AssignWinnerAsync(Guid eventInstanceId, Guid userId, int rank);
}
