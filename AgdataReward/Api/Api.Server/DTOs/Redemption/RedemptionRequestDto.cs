using Domain.Enums;

namespace Api.Server.DTOs.Redemption;

public class RedemptionRequestDto
{
    public Guid Id { get; set; }
    public Guid RedemptionId { get; set; }
    public int PointsUsed { get; set; }
    public RedemptionStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
}
