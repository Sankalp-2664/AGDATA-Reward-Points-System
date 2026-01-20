using Api.Server.DTOs.Event;
using Application.Interfaces;
using AutoMapper;
using Domain.Entities.Event;
using Domain.Entities.Reward;

namespace Api.Server.Services;

public interface IEventApiService
{
    Task<EventDefinitionDto> CreateEventAsync(EventDefinitionCreateDto dto, CancellationToken cancellationToken = default);
    Task<IEnumerable<EventDefinitionDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<EventWithRewardsDto>> GetAllWithRewardsAsync(Guid? currentUserId = null, CancellationToken cancellationToken = default);
    Task<EventDefinitionDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<EventDefinitionDto?> UpdateAsync(Guid id, EventDefinitionUpdateDto dto, CancellationToken cancellationToken = default);
    Task<EventDefinitionDto> UpdateEventStatusAsync(Guid eventId, string status, CancellationToken cancellationToken = default);
    Task CompleteEventWithWinnersAsync(Guid eventId, Guid? firstPlaceUserId, Guid? secondPlaceUserId, Guid? thirdPlaceUserId, CancellationToken cancellationToken = default);
    Task AddRewardRuleAsync(Guid eventId, int rank, Guid rewardPointsId, CancellationToken cancellationToken = default);
    Task UpdateRewardRuleAsync(Guid ruleId, Guid rewardPointsId, CancellationToken cancellationToken = default);
    Task AssignWinnerAsync(Guid eventInstanceId, Guid userId, int rank, CancellationToken cancellationToken = default);
    Task ParticipateAsync(Guid eventInstanceId, Guid userId, CancellationToken cancellationToken = default);
}

public class EventApiService(
    IEventService eventService,
    IEventInstanceRepository instanceRepo,
    IRewardPointsRepository rewardPointsRepo,
    IMapper mapper) : IEventApiService
{
    private readonly IEventService _eventService = eventService;
    private readonly IEventInstanceRepository _instanceRepo = instanceRepo;
    private readonly IRewardPointsRepository _rewardPointsRepo = rewardPointsRepo;
    private readonly IMapper _mapper = mapper;

    /// <summary>
    /// Gets or creates a reward points entry for the given value.
    /// </summary>
    private async Task<Guid> GetOrCreateRewardPointsIdAsync(int pointsValue)
    {
        // Try to find existing reward points with this value
        var existing = await _rewardPointsRepo.GetByValueAsync(pointsValue);
        if (existing != null)
        {
            Console.WriteLine($"    ✅ Found existing RewardPoints: {existing.Id} = {pointsValue} points");
            return existing.Id;
        }
        
        // Create new reward points
        var newRewardPoints = new RewardPoints(Guid.NewGuid(), pointsValue);
        await _rewardPointsRepo.AddAsync(newRewardPoints);
        Console.WriteLine($"    ➕ Created new RewardPoints: {newRewardPoints.Id} = {pointsValue} points");
        return newRewardPoints.Id;
    }

    public async Task<EventDefinitionDto> CreateEventAsync(
        EventDefinitionCreateDto dto,
        CancellationToken cancellationToken = default)
    {
        // mapping stays here, not in controller
        var domain = _mapper.Map<EventDefinition>(dto);

        var created = await _eventService.CreateEventAsync(domain.Code, domain.Title, domain.StartDate, domain.EndDate);
        Console.WriteLine($"🎉 Event created: {created.Id} - {created.Code}");
        
        // Process prize values - either direct values or reward points IDs
        // First Prize
        if (dto.FirstPrize.HasValue && dto.FirstPrize.Value > 0)
        {
            Console.WriteLine($"  🥇 Processing 1st prize: {dto.FirstPrize.Value} points");
            var rewardPointsId = await GetOrCreateRewardPointsIdAsync(dto.FirstPrize.Value);
            await _eventService.AddRewardRuleAsync(created.Id, 1, rewardPointsId);
        }
        else if (dto.FirstPrizeRewardPointsId.HasValue && dto.FirstPrizeRewardPointsId.Value != Guid.Empty)
        {
            Console.WriteLine($"  🥇 Creating reward rule rank 1: RewardPointsId = {dto.FirstPrizeRewardPointsId.Value}");
            await _eventService.AddRewardRuleAsync(created.Id, 1, dto.FirstPrizeRewardPointsId.Value);
        }
        
        // Second Prize
        if (dto.SecondPrize.HasValue && dto.SecondPrize.Value > 0)
        {
            Console.WriteLine($"  🥈 Processing 2nd prize: {dto.SecondPrize.Value} points");
            var rewardPointsId = await GetOrCreateRewardPointsIdAsync(dto.SecondPrize.Value);
            await _eventService.AddRewardRuleAsync(created.Id, 2, rewardPointsId);
        }
        else if (dto.SecondPrizeRewardPointsId.HasValue && dto.SecondPrizeRewardPointsId.Value != Guid.Empty)
        {
            Console.WriteLine($"  🥈 Creating reward rule rank 2: RewardPointsId = {dto.SecondPrizeRewardPointsId.Value}");
            await _eventService.AddRewardRuleAsync(created.Id, 2, dto.SecondPrizeRewardPointsId.Value);
        }
        
        // Third Prize
        if (dto.ThirdPrize.HasValue && dto.ThirdPrize.Value > 0)
        {
            Console.WriteLine($"  🥉 Processing 3rd prize: {dto.ThirdPrize.Value} points");
            var rewardPointsId = await GetOrCreateRewardPointsIdAsync(dto.ThirdPrize.Value);
            await _eventService.AddRewardRuleAsync(created.Id, 3, rewardPointsId);
        }
        else if (dto.ThirdPrizeRewardPointsId.HasValue && dto.ThirdPrizeRewardPointsId.Value != Guid.Empty)
        {
            Console.WriteLine($"  🥉 Creating reward rule rank 3: RewardPointsId = {dto.ThirdPrizeRewardPointsId.Value}");
            await _eventService.AddRewardRuleAsync(created.Id, 3, dto.ThirdPrizeRewardPointsId.Value);
        }
        
        return _mapper.Map<EventDefinitionDto>(created);
    }

    public async Task<IEnumerable<EventDefinitionDto>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        var events = await _eventService.ListEventsAsync();
        return _mapper.Map<IEnumerable<EventDefinitionDto>>(events);
    }

    public async Task<IEnumerable<EventWithRewardsDto>> GetAllWithRewardsAsync(
        Guid? currentUserId = null,
        CancellationToken cancellationToken = default)
    {
        var events = await _eventService.ListEventsAsync();
        var result = new List<EventWithRewardsDto>();
        
        Console.WriteLine($"📊 GetAllWithRewardsAsync: Found {events.Count()} events");

        foreach (var evt in events)
        {
            Console.WriteLine($"📝 Processing event: {evt.Id} - {evt.Code}");
            var rewardRules = new List<RewardRuleWithPointsDto>();
            
            for (int rank = 1; rank <= 3; rank++)
            {
                var rule = await _eventService.GetRewardRuleByEventAndRankAsync(evt.Id, rank);
                Console.WriteLine($"  - Rank {rank}: {(rule != null ? $"Found rule {rule.Id}, RewardPointsId: {rule.RewardPointsId}, Points: {rule.RewardPoints?.PointsValue}" : "No rule")}");
                if (rule != null)
                {
                    rewardRules.Add(new RewardRuleWithPointsDto
                    {
                        Id = rule.Id,
                        Rank = rule.Rank,
                        RewardPointsId = rule.RewardPointsId,
                        PointsValue = rule.RewardPoints?.PointsValue ?? 0
                    });
                }
            }
            
            Console.WriteLine($"  - Total rules for event: {rewardRules.Count}");
            
            // Get participants count
            var participantsCount = await _eventService.GetParticipantsCountAsync(evt.Id);
            Console.WriteLine($"  - Participants count: {participantsCount}");
            
            // Check if current user has participated
            bool isParticipated = false;
            if (currentUserId.HasValue && currentUserId.Value != Guid.Empty)
            {
                var instances = await _instanceRepo.GetByEventIdAsync(evt.Id);
                isParticipated = instances?.Any(i => i.ParticipantIds.Contains(currentUserId.Value)) ?? false;
                Console.WriteLine($"  - User {currentUserId.Value} participated: {isParticipated}");
            }
            
            // Check if winners have been assigned (by checking for transactions)
            var winnersAssigned = await _eventService.HasWinnersAssignedAsync(evt.Id);
            Console.WriteLine($"  - Winners assigned: {winnersAssigned}");
            
            // Use computed status based on dates
            var computedStatus = evt.GetComputedStatus();
            Console.WriteLine($"  - Computed status: {computedStatus} (stored: {evt.Status})");

            result.Add(new EventWithRewardsDto
            {
                Id = evt.Id,
                Code = evt.Code,
                Title = evt.Title,
                StartDate = evt.StartDate,
                EndDate = evt.EndDate,
                Status = computedStatus,
                ParticipantsCount = participantsCount,
                WinnersAssigned = winnersAssigned,
                IsParticipated = isParticipated,
                RewardRules = rewardRules
            });
        }

        return result;
    }

    public async Task<EventDefinitionDto?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var entity = await _eventService.GetEventByIdAsync(id);
        return entity is null
            ? null
            : _mapper.Map<EventDefinitionDto>(entity);
    }

    public async Task<EventDefinitionDto?> UpdateAsync(
        Guid id,
        EventDefinitionUpdateDto dto,
        CancellationToken cancellationToken = default)
    {
        if (id != dto.Id)
            throw new ArgumentException("Event ID mismatch.", nameof(id));

        // Use Name as alias for Title if Title is not provided
        var title = dto.Title ?? dto.Name;
        
        var updated = await _eventService.UpdateEventAsync(id, dto.Code, title, dto.StartDate, dto.EndDate, dto.Status);
        Console.WriteLine($"🔄 Event updated: {id} - {updated.Code}");
        
        // Update or create reward rules if provided (direct values or IDs)
        await UpdateOrCreateRewardRuleForPrizeAsync(id, 1, dto.FirstPrize, dto.FirstPrizeRewardPointsId);
        await UpdateOrCreateRewardRuleForPrizeAsync(id, 2, dto.SecondPrize, dto.SecondPrizeRewardPointsId);
        await UpdateOrCreateRewardRuleForPrizeAsync(id, 3, dto.ThirdPrize, dto.ThirdPrizeRewardPointsId);
        
        return _mapper.Map<EventDefinitionDto>(updated);
    }
    
    private async Task UpdateOrCreateRewardRuleForPrizeAsync(Guid eventId, int rank, int? prizeValue, Guid? rewardPointsId)
    {
        Guid? targetRewardPointsId = null;
        
        // Prefer direct prize value over reward points ID
        if (prizeValue.HasValue && prizeValue.Value > 0)
        {
            targetRewardPointsId = await GetOrCreateRewardPointsIdAsync(prizeValue.Value);
            Console.WriteLine($"    🏆 Rank {rank}: Using prize value {prizeValue.Value} -> RewardPointsId: {targetRewardPointsId}");
        }
        else if (rewardPointsId.HasValue && rewardPointsId.Value != Guid.Empty)
        {
            targetRewardPointsId = rewardPointsId.Value;
            Console.WriteLine($"    🏆 Rank {rank}: Using provided RewardPointsId: {targetRewardPointsId}");
        }
        
        if (!targetRewardPointsId.HasValue)
        {
            Console.WriteLine($"    ⏭️ Rank {rank}: No prize value or RewardPointsId provided, skipping");
            return;
        }
        
        await UpdateOrCreateRewardRuleAsync(eventId, rank, targetRewardPointsId);
    }
    
    private async Task UpdateOrCreateRewardRuleAsync(Guid eventId, int rank, Guid? rewardPointsId)
    {
        if (!rewardPointsId.HasValue || rewardPointsId.Value == Guid.Empty)
            return;
            
        // Check if rule already exists for this event and rank
        var existingRule = await _eventService.GetRewardRuleByEventAndRankAsync(eventId, rank);
        
        if (existingRule != null)
        {
            // Update existing rule if reward points changed
            if (existingRule.RewardPointsId != rewardPointsId.Value)
            {
                await _eventService.UpdateRewardRuleAsync(existingRule.Id, rewardPointsId.Value);
            }
        }
        else
        {
            // Create new rule
            await _eventService.AddRewardRuleAsync(eventId, rank, rewardPointsId.Value);
        }
    }

    public async Task AssignWinnerAsync(
        Guid eventInstanceId,
        Guid userId,
        int rank,
        CancellationToken cancellationToken = default)
    {
        await _eventService.AssignWinnerAsync(eventInstanceId, userId, rank);
    }

    public async Task AddRewardRuleAsync(
        Guid eventId,
        int rank,
        Guid rewardPointsId,
        CancellationToken cancellationToken = default)
    {
        await _eventService.AddRewardRuleAsync(eventId, rank, rewardPointsId);
    }

    public async Task UpdateRewardRuleAsync(
        Guid ruleId,
        Guid rewardPointsId,
        CancellationToken cancellationToken = default)
    {
        await _eventService.UpdateRewardRuleAsync(ruleId, rewardPointsId);
    }

    public async Task ParticipateAsync(
    Guid eventInstanceId,
    Guid userId,
    CancellationToken cancellationToken = default)
    {
        // Use the new method that accepts EventDefinition ID
        await _eventService.ParticipateInEventDefinitionAsync(eventInstanceId, userId);
    }

    public async Task<EventDefinitionDto> UpdateEventStatusAsync(
        Guid eventId,
        string status,
        CancellationToken cancellationToken = default)
    {
        var updated = await _eventService.UpdateEventStatusAsync(eventId, status);
        return _mapper.Map<EventDefinitionDto>(updated);
    }

    public async Task CompleteEventWithWinnersAsync(
        Guid eventId,
        Guid? firstPlaceUserId,
        Guid? secondPlaceUserId,
        Guid? thirdPlaceUserId,
        CancellationToken cancellationToken = default)
    {
        await _eventService.CompleteEventWithWinnersAsync(eventId, firstPlaceUserId, secondPlaceUserId, thirdPlaceUserId);
    }
}
