using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities.Reward
{
    /// <summary>
    /// Represents a defined reward points value used for events or product redemptions.
    /// </summary>
    public class RewardPoints
    {
        public Guid Id { get; } // Primary key
        public int PointsValue { get; } // The value of the reward points

        protected RewardPoints() { } // For ORM
        public RewardPoints(Guid id, int pointsValue)
        {
            if (id == Guid.Empty)
                throw new ArgumentException("Id cannot be empty.", nameof(id));
            if (pointsValue <= 0)
                throw new ArgumentOutOfRangeException(nameof(pointsValue), "Points must be greater than zero.");

            Id = id;
            PointsValue = pointsValue;
        }
    }
}