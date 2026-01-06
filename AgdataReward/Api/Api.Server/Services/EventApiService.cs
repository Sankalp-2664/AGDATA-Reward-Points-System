using Api.Server.DTOs.Event;
using Application.Interfaces;
using AutoMapper;
using Domain.Entities.Event;

namespace Api.Server.Services;

public interface IEventApiService
{
    Task<EventDefinitionDto> CreateEventAsync(EventDefinitionCreateDto dto, CancellationToken cancellationToken = default);
    Task<IEnumerable<EventDefinitionDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<EventDefinitionDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<EventDefinitionDto?> UpdateAsync(Guid id, EventDefinitionUpdateDto dto, CancellationToken cancellationToken = default);

    Task AssignWinnerAsync(Guid eventInstanceId, Guid userId, int rank, CancellationToken cancellationToken = default);
    Task AddRewardRuleAsync(Guid eventId, Guid rewardPointsId, int rank, CancellationToken cancellationToken = default);
    Task ParticipateAsync(Guid eventInstanceId, Guid userId, CancellationToken cancellationToken = default);
}

public class EventApiService(
    IEventService eventService,
    IMapper mapper) : IEventApiService
{
    private readonly IEventService _eventService = eventService;
    private readonly IMapper _mapper = mapper;

    public async Task<EventDefinitionDto> CreateEventAsync(
        EventDefinitionCreateDto dto,
        CancellationToken cancellationToken = default)
    {
        // mapping stays here, not in controller
        var domain = _mapper.Map<EventDefinition>(dto);

        var created = await _eventService.CreateEventAsync(domain.Code, domain.Title);
        return _mapper.Map<EventDefinitionDto>(created);
    }

    public async Task<IEnumerable<EventDefinitionDto>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        var events = await _eventService.ListEventsAsync();
        return _mapper.Map<IEnumerable<EventDefinitionDto>>(events);
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

        var existing = await _eventService.GetEventByIdAsync(id);
        if (existing is null)
            return null;

        _mapper.Map(dto, existing);
        var saved = await _eventService.UpdateEventAsync(existing);

        return _mapper.Map<EventDefinitionDto>(saved);
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
        Guid rewardPointsId,
        int rank,
        CancellationToken cancellationToken = default)
    {
        await _eventService.AddRewardRuleAsync(eventId, rank, rewardPointsId);
    }

    public async Task ParticipateAsync(
    Guid eventInstanceId,
    Guid userId,
    CancellationToken cancellationToken = default)
    {
        await _eventService.ParticipateAsync(eventInstanceId, userId);
    }
}
