namespace Api.Server.DTOs.Event;

public class EventInstanceDto
{
    public Guid Id { get; set; }
    public Guid EventId { get; set; }
    public int? Rank { get; set; }
    public Guid? WinnerUserId { get; set; }
}
