using System;
using Domain.Entities.Reward;
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

    public static RewardTransactionDto FromDomain(RewardTransaction entity)
    {
        return new RewardTransactionDto
        {
            Id = entity.Id,
            UserId = entity.UserId,
            PointsDelta = entity.PointsDelta,
            Notes = entity.Notes,
            TransactionType = entity.TransactionType,
            CreatedAt = entity.CreatedAt,
            EventId = entity.EventId,
            RedemptionId = entity.RedemptionId
        };
    }
}
