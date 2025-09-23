using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities;

public sealed class RewardPoints
{
    public Guid Id { get; }
    public int PointsValue { get; }

    public RewardPoints(Guid id, int pointsValue)
    {
        if (pointsValue <= 0) throw new ArgumentException("Points must be greater than zero.");
        Id = id;
        PointsValue = pointsValue;
    }
}