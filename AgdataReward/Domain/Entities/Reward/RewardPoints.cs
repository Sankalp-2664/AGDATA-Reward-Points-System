namespace Domain.Entities.Reward;

/// <summary>
/// Represents a defined reward points value used for events or product redemptions.
/// </summary>
public class RewardPoints
{
    public Guid Id { get; private set; } // Primary key
    public int PointsValue { get; private set; } // The value of the reward points

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

    /// <summary>
    /// Updates the points value.
    /// </summary>
    /// <param name="newPointsValue">The new points value.</param>
    public void UpdatePointsValue(int newPointsValue)
    {
        if (newPointsValue <= 0)
            throw new ArgumentOutOfRangeException(nameof(newPointsValue), "Points must be greater than zero.");

        PointsValue = newPointsValue;
    }
}