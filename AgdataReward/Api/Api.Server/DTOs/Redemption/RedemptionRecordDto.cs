using Domain.Entities.Redemption;

namespace Api.Server.DTOs.Redemption;

public class RedemptionRecordDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid ProductId { get; set; }
    public DateTime RedeemedAt { get; set; }

    public static RedemptionRecordDto FromDomain(RedemptionRecord entity)
    {
        return new RedemptionRecordDto
        {
            Id = entity.Id,
            UserId = entity.UserId,
            ProductId = entity.ProductId,
            RedeemedAt = entity.RedeemedAt
        };
    }
}
