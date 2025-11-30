using Domain.Enums;

namespace Api.Server.DTOs.Reward;

public class RewardTransactionDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public int PointsDelta { get; set; }
    public string Notes { get; set; } = string.Empty;
    public TransactionType TransactionType { get; set; }
    public DateTime CreatedAt { get; set; }
    public Guid? EventId { get; set; }
    public Guid? RedemptionId { get; set; }
}
