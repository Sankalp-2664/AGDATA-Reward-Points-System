using Domain.Enums;

namespace Domain.Entities.Redemption;

/// <summary>
/// Represents the lifecycle of a reward redemption, including approval and completion.
/// </summary>
public class RedemptionRequest
{
    public Guid Id { get; private set; } // Primary Key
    public Guid RedemptionId { get; private set; } // Foreign Key to Redemption (Redemption.Id)
    public int PointsUsed { get; private set; } // Points used for this redemption
    public RedemptionStatus Status { get; private set; } // Current status of the redemption
    public DateTime CreatedAt { get; private set; } // Timestamp when the request was created
    protected RedemptionRequest() { } // For ORM

    public RedemptionRequest(Guid redemptionId, int pointsUsed)
    {
        if (pointsUsed <= 0)
            throw new ArgumentException("PointsUsed must be positive.");

        Id = Guid.NewGuid(); // Unique ID for request
        RedemptionId = redemptionId; // Tied to the redemption record
        PointsUsed = pointsUsed;
        Status = RedemptionStatus.Pending;
        CreatedAt = DateTime.UtcNow;
    }

    public void Approve() // Approve the redemption request
    {
        EnsureStatus(RedemptionStatus.Pending, "approved");
        Status = RedemptionStatus.Approved;
    }

    public void Reject() // Reject the redemption request
    {
        EnsureStatus(RedemptionStatus.Pending, "rejected");
        Status = RedemptionStatus.Rejected;
    }

    public void MarkCompleted() // Mark the redemption as completed
    {
        EnsureStatus(RedemptionStatus.Approved, "completed");
        Status = RedemptionStatus.Completed;
    }

    private void EnsureStatus(RedemptionStatus requiredStatus, string action) // Validate current status before action
    {
        if (Status != requiredStatus)
            throw new InvalidOperationException($"Only {requiredStatus} redemptions can be {action}.");
    }

    /// <summary>
    /// Returns the list of valid next statuses based on current status.
    /// </summary>
    public RedemptionStatus[] GetAllowedTransitions()
    {
        return Status switch
        {
            RedemptionStatus.Pending => new[] { RedemptionStatus.Approved, RedemptionStatus.Rejected },
            RedemptionStatus.Approved => new[] { RedemptionStatus.Completed },
            _ => Array.Empty<RedemptionStatus>()
        };
    }
}