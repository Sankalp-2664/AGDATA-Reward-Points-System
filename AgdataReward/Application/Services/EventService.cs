using Application.Interfaces;
using Domain.Entities.Event;
using Domain.Entities.Reward;
using Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace Application.Services;

public class EventService(
    IEventDefinitionRepository definitionRepo,
    IEventRewardRuleRepository ruleRepo,
    IEventInstanceRepository instanceRepo,
    IUserAccountRepository accountRepo,
    IRewardTransactionRepository transactionRepo,
    IRewardPointsRepository rewardPointsRepo) : IEventService
{
    private readonly IEventDefinitionRepository _definitionRepo = definitionRepo;
    private readonly IEventRewardRuleRepository _ruleRepo = ruleRepo;
    private readonly IEventInstanceRepository _instanceRepo = instanceRepo;
    private readonly IUserAccountRepository _accountRepo = accountRepo;
    private readonly IRewardTransactionRepository _transactionRepo = transactionRepo;
    private readonly IRewardPointsRepository _rewardPointsRepo = rewardPointsRepo;

    public async Task<EventDefinition> CreateEventAsync(string code, string title, DateTime startDate, DateTime endDate)
    {
        var existing = await _definitionRepo.ListAsync();
        if (existing.Any(e => e.Code.Equals(code, StringComparison.OrdinalIgnoreCase)))
            throw new InvalidOperationException("Duplicate event code not allowed");

        var definition = new EventDefinition(Guid.NewGuid(), code, title, startDate, endDate);
        await _definitionRepo.AddAsync(definition);
        return definition;
    }

    public async Task AddRewardRuleAsync(Guid eventId, int rank, Guid rewardPointsId)
    {
        var rule = new EventRewardRule(Guid.NewGuid(), eventId, rank, rewardPointsId);
        await _ruleRepo.AddAsync(rule);
    }

    public async Task UpdateRewardRuleAsync(Guid ruleId, Guid rewardPointsId)
    {
        var rule = await _ruleRepo.GetByIdAsync(ruleId);
        if (rule == null)
            throw new ArgumentException("Reward rule not found.");
        
        rule.UpdateRewardPoints(rewardPointsId);
        await _ruleRepo.UpdateAsync(rule);
    }

    public async Task DeleteRewardRuleAsync(Guid ruleId)
    {
        await _ruleRepo.DeleteAsync(ruleId);
    }

    public async Task<EventRewardRule?> GetRewardRuleByEventAndRankAsync(Guid eventId, int rank)
    {
        var rules = await _ruleRepo.GetByEventIdAsync(eventId);
        return rules.FirstOrDefault(r => r.Rank == rank);
    }

    public async Task<EventDefinition?> GetEventByIdAsync(Guid id)
    {
        if (id == Guid.Empty)
            throw new ValidationException("Event ID cannot be empty.");
        return await _definitionRepo.GetByIdAsync(id);
    }

    public async Task<IEnumerable<EventDefinition>> ListEventsAsync()
    {
        return await _definitionRepo.ListAsync();
    }

    public async Task<EventDefinition> UpdateEventAsync(Guid id, string? code, string? title, DateTime? startDate, DateTime? endDate, string? status = null)
    {
        var eventDef = await _definitionRepo.GetByIdAsync(id);
        if (eventDef == null)
            throw new ArgumentException("Event not found.");

        // Update the event details using domain method
        eventDef.UpdateDetails(code, title, startDate, endDate);
        
        // Update status if provided
        if (!string.IsNullOrWhiteSpace(status))
        {
            eventDef.UpdateStatus(status);
        }
        
        await _definitionRepo.UpdateAsync(eventDef);
        return await _definitionRepo.GetByIdAsync(id) ?? eventDef;
    }


    /// <summary>
    /// eventInstanceId = EventInstance.Id (the occurrence). Assigns a winner for that occurrence.
    /// </summary>
    public async Task AssignWinnerAsync(Guid eventInstanceId, Guid userId, int rank)
    {

        // 1) load the event instance (occurrence)
        var instance = await _instanceRepo.GetByIdAsync(eventInstanceId)
            ?? throw new ArgumentException("Invalid event instance id.");

        // 2) load rules for the event definition (instance.EventId)
        var rules = await _ruleRepo.GetByEventIdAsync(instance.EventId);
        var rewardRule = rules.FirstOrDefault(r => r.Rank == rank);
        if (rewardRule == null)
            throw new ArgumentException("No reward rule for this rank.");

        // 3) resolve numeric points from RewardPoints
        var rewardPoints = await _rewardPointsRepo.GetByIdAsync(rewardRule.RewardPointsId)
            ?? throw new ArgumentException("Reward points configuration not found.");
        var pointsToAward = rewardPoints.PointsValue;

        // 4) get user account
        var account = await _accountRepo.GetByUserIdAsync(userId)
            ?? throw new ArgumentException("Invalid user.");

        // 5) create transaction and persist it
        var transaction = new RewardTransaction(
            userId,
            rewardPoints.PointsValue,
            $"Earned from event {instance.EventId} (instance {instance.Id})",
            TransactionType.Credit,
            eventId: instance.EventId
        );

        await _transactionRepo.AddAsync(transaction);

        // 6) update account through domain method (which also records the transaction inside the aggregate)
        account.AddPoints(rewardPoints.PointsValue, transaction);
        await _accountRepo.UpdateAsync(account);
    }

    public async Task ParticipateAsync(Guid eventInstanceId, Guid userId)
    {
        if (eventInstanceId == Guid.Empty)
            throw new ArgumentException("Invalid event instance id.");

        var instance = await _instanceRepo.GetByIdAsync(eventInstanceId)
            ?? throw new ArgumentException("Event instance not found.");

        instance.AddParticipant(userId);

        await _instanceRepo.UpdateAsync(instance);
    }

    public async Task ParticipateInEventDefinitionAsync(Guid eventDefinitionId, Guid userId)
    {
        if (eventDefinitionId == Guid.Empty)
            throw new ArgumentException("Invalid event id.");

        // Get or create an event instance for this event definition
        var instances = await _instanceRepo.GetByEventIdAsync(eventDefinitionId);
        EventInstance instance;

        if (instances == null || !instances.Any())
        {
            // Create a new event instance for this event
            instance = new EventInstance(Guid.NewGuid(), eventDefinitionId);
            await _instanceRepo.AddAsync(instance);
        }
        else
        {
            // Use the first instance (or you could create logic to find the right one)
            instance = instances.First();
        }

        // Add participant to the instance
        instance.AddParticipant(userId);
        await _instanceRepo.UpdateAsync(instance);
    }

    public async Task<EventDefinition> UpdateEventStatusAsync(Guid eventId, string status)
    {
        var eventDef = await _definitionRepo.GetByIdAsync(eventId)
            ?? throw new ArgumentException("Event not found.");
        
        eventDef.UpdateStatus(status);
        await _definitionRepo.UpdateAsync(eventDef);
        return eventDef;
    }

    public async Task CompleteEventWithWinnersAsync(Guid eventId, Guid? firstPlaceUserId, Guid? secondPlaceUserId, Guid? thirdPlaceUserId)
    {
        // 1) Update event status to Completed
        var eventDef = await _definitionRepo.GetByIdAsync(eventId)
            ?? throw new ArgumentException("Event not found.");
        
        eventDef.UpdateStatus("Completed");
        await _definitionRepo.UpdateAsync(eventDef);

        // 2) Award points to winners
        var rules = await _ruleRepo.GetByEventIdAsync(eventId);

        // Award 1st place
        if (firstPlaceUserId.HasValue && firstPlaceUserId.Value != Guid.Empty)
        {
            await AwardPointsToWinnerAsync(eventId, firstPlaceUserId.Value, 1, rules);
        }

        // Award 2nd place
        if (secondPlaceUserId.HasValue && secondPlaceUserId.Value != Guid.Empty)
        {
            await AwardPointsToWinnerAsync(eventId, secondPlaceUserId.Value, 2, rules);
        }

        // Award 3rd place
        if (thirdPlaceUserId.HasValue && thirdPlaceUserId.Value != Guid.Empty)
        {
            await AwardPointsToWinnerAsync(eventId, thirdPlaceUserId.Value, 3, rules);
        }
    }

    private async Task AwardPointsToWinnerAsync(Guid eventId, Guid userId, int rank, IEnumerable<EventRewardRule> rules)
    {
        var rewardRule = rules.FirstOrDefault(r => r.Rank == rank);
        if (rewardRule == null)
        {
            Console.WriteLine($"⚠️ No reward rule found for rank {rank} in event {eventId}");
            return;
        }

        var rewardPoints = await _rewardPointsRepo.GetByIdAsync(rewardRule.RewardPointsId);
        if (rewardPoints == null)
        {
            Console.WriteLine($"⚠️ Reward points not found for RewardPointsId {rewardRule.RewardPointsId}");
            return;
        }

        var account = await _accountRepo.GetByUserIdAsync(userId);
        if (account == null)
        {
            Console.WriteLine($"⚠️ User account not found for user {userId}");
            return;
        }

        // Get event name for better description
        var eventDef = await _definitionRepo.GetByIdAsync(eventId);
        var eventName = eventDef?.Title ?? "Unknown Event";

        // Note: RewardTransaction.UserId is FK to UserAccount.Id (not UserProfile.Id)
        // So we use account.Id here, not userId (which is UserProfile.Id)
        // eventId is null because RewardTransaction.EventId references EventInstance (not EventDefinition)
        // The event info is stored in the Notes field instead
        var transaction = new RewardTransaction(
            account.Id,  // Use UserAccount.Id (FK target), not UserProfile.Id
            rewardPoints.PointsValue,
            $"Won {rank}{GetOrdinalSuffix(rank)} place in {eventName}",
            TransactionType.Credit,
            eventId: null  // EventId references EventInstance, not EventDefinition
        );

        await _transactionRepo.AddAsync(transaction);
        account.AddPoints(rewardPoints.PointsValue, transaction);
        await _accountRepo.UpdateAsync(account);

        Console.WriteLine($"✅ Awarded {rewardPoints.PointsValue} points to user {userId} for {rank}{GetOrdinalSuffix(rank)} place");
    }

    private static string GetOrdinalSuffix(int rank) => rank switch
    {
        1 => "st",
        2 => "nd",
        3 => "rd",
        _ => "th"
    };

    public async Task<int> GetParticipantsCountAsync(Guid eventId)
    {
        var instances = await _instanceRepo.GetByEventIdAsync(eventId);
        return instances.Sum(i => i.ParticipantIds.Count);
    }

    public async Task<bool> HasWinnersAssignedAsync(Guid eventId)
    {
        return await _transactionRepo.HasTransactionsForEventAsync(eventId);
    }
}
