namespace Api.Server.DTOs.Event;

public class EventRewardRuleDto
{
    public Guid Id { get; set; }
    public Guid EventId { get; set; }
    public int Rank { get; set; }
    public Guid RewardPointsId { get; set; }
}
