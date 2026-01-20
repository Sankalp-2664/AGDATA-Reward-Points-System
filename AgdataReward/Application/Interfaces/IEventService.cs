using Domain.Entities.Event;

namespace Application.Interfaces;

public interface IEventService
{
    Task<EventDefinition> CreateEventAsync(string code, string title, DateTime startDate, DateTime endDate);
    Task<EventDefinition?> GetEventByIdAsync(Guid id);
    Task<IEnumerable<EventDefinition>> ListEventsAsync();
    Task<EventDefinition> UpdateEventAsync(Guid id, string? code, string? title, DateTime? startDate, DateTime? endDate, string? status = null);
    Task<EventDefinition> UpdateEventStatusAsync(Guid eventId, string status);
    Task AddRewardRuleAsync(Guid eventId, int rank, Guid rewardPointsId);
    Task UpdateRewardRuleAsync(Guid ruleId, Guid rewardPointsId);
    Task DeleteRewardRuleAsync(Guid ruleId);
    Task<EventRewardRule?> GetRewardRuleByEventAndRankAsync(Guid eventId, int rank);
    Task AssignWinnerAsync(Guid eventInstanceId, Guid userId, int rank);
    Task CompleteEventWithWinnersAsync(Guid eventId, Guid? firstPlaceUserId, Guid? secondPlaceUserId, Guid? thirdPlaceUserId);
    Task ParticipateAsync(Guid eventInstanceId, Guid userId);
    Task ParticipateInEventDefinitionAsync(Guid eventDefinitionId, Guid userId);
    Task<int> GetParticipantsCountAsync(Guid eventId);
    Task<bool> HasWinnersAssignedAsync(Guid eventId);
}
