using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities;

public class EventRewardRule
{
    public Guid Id { get; }
    public Guid EventId { get; }
    public int Rank { get; }
    public Guid RewardPointsId { get; private set; }

    public EventRewardRule(Guid id, Guid eventId, int rank, Guid rewardPointsId)
    {
        Id = id;
        EventId = eventId;
        Rank = rank;
        RewardPointsId = rewardPointsId;
    }

    public void UpdateRewardPoints(Guid newRewardPointsId)
    {
        RewardPointsId = newRewardPointsId;
    }
}

