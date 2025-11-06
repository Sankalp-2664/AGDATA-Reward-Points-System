using Domain.Entities.Redemption;
using Domain.Enums;

namespace Api.Server.DTOs.Redemption;

public class RedemptionRequestDto
{
    public Guid Id { get; set; }
    public Guid RedemptionId { get; set; }
    public int PointsUsed { get; set; }
    public RedemptionStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }

    public static RedemptionRequestDto FromDomain(RedemptionRequest entity)
    {
        return new RedemptionRequestDto
        {
            Id = entity.Id,
            RedemptionId = entity.RedemptionId,
            PointsUsed = entity.PointsUsed,
            Status = entity.Status,
            CreatedAt = entity.CreatedAt
        };
    }
}
