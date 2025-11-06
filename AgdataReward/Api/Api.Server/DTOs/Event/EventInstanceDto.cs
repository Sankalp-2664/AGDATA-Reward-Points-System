using Domain.Entities.Event;

namespace Api.Server.DTOs.Event;

public class EventInstanceDto
{
    public Guid Id { get; set; }
    public Guid EventId { get; set; }
    public int? Rank { get; set; }
    public Guid? WinnerUserId { get; set; }

    public static EventInstanceDto FromDomain(EventInstance entity)
    {
        return new EventInstanceDto
        {
            Id = entity.Id,
            EventId = entity.EventId,
            Rank = entity.Rank,
            WinnerUserId = entity.WinnerUserId
        };
    }
}
