using Domain.Enums;

namespace Api.Server.DTOs.Redemption;

/// <summary>
/// DTO for pending redemption requests with user and product details
/// </summary>
public class PendingRedemptionDto
{
    public Guid Id { get; set; }
    public Guid RedemptionId { get; set; }
    public Guid UserId { get; set; }
    public string EmployeeId { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string UserEmail { get; set; } = string.Empty;
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public int PointsUsed { get; set; }
    public RedemptionStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime RedeemedAt { get; set; }
}
