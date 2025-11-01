using Domain.Entities.Reward;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities.Event
{
    /// <summary>
    /// Defines the points reward for a specific rank in an event.
    /// </summary>
    public class EventRewardRule
    {
        public Guid Id { get; private set; } // Primary Key
        public Guid EventId { get; private set; } // Foreign Key to EventDefinition (EventDefinition.Id)
        public int Rank { get; private set; } // Rank position (1 for first place, 2 for second, etc.)
        public Guid RewardPointsId { get; private set; } // Foreign Key to RewardPoints (RewardPoints.Id)
        
        public virtual EventDefinition? Event { get; private set; } // Navigation property to EventDefinition
        public virtual RewardPoints? RewardPoints { get; private set; } // Navigation property to RewardPoints

        protected EventRewardRule() { } // For ORM

        public EventRewardRule(Guid id, Guid eventId, int rank, Guid rewardPointsId)
        {
            if (id == Guid.Empty) throw new ArgumentException("Id cannot be empty.", nameof(id));
            if (eventId == Guid.Empty) throw new ArgumentException("EventId cannot be empty.", nameof(eventId));
            if (rank <= 0) throw new ArgumentOutOfRangeException(nameof(rank), "Rank must be greater than zero.");
            if (rewardPointsId == Guid.Empty) throw new ArgumentException("RewardPointsId cannot be empty.", nameof(rewardPointsId));

            Id = id;
            EventId = eventId;
            Rank = rank;
            RewardPointsId = rewardPointsId;
        }

        /// <summary>
        /// Updates reward points linked to this rank.
        /// </summary>
        public void UpdateRewardPoints(Guid newRewardPointsId)
        {
            if (newRewardPointsId == Guid.Empty)
                throw new ArgumentException("New RewardPointsId cannot be empty.", nameof(newRewardPointsId));

            RewardPointsId = newRewardPointsId;
        }

    }
}

