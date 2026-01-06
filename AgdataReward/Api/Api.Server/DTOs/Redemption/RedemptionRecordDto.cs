namespace Api.Server.DTOs.Redemption;

public class RedemptionRecordDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid ProductId { get; set; }
    public DateTime RedeemedAt { get; set; }
}
