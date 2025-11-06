using Domain.Entities.Event;

namespace Api.Server.DTOs.Event;

public class EventRewardRuleDto
{
    public Guid Id { get; set; }
    public Guid EventId { get; set; }
    public int Rank { get; set; }
    public Guid RewardPointsId { get; set; }

    public static EventRewardRuleDto FromDomain(EventRewardRule entity)
    {
        return new EventRewardRuleDto
        {
            Id = entity.Id,
            EventId = entity.EventId,
            Rank = entity.Rank,
            RewardPointsId = entity.RewardPointsId
        };
    }
}
