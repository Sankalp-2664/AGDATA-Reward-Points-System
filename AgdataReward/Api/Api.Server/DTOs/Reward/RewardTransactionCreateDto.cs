using Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace Api.Server.DTOs.Reward;

public class RewardTransactionCreateDto
{
    [Required]
    public Guid UserId { get; set; }

    [Required]
    [Range(int.MinValue, int.MaxValue)]
    public int PointsDelta { get; set; }

    [Required]
    [StringLength(500)]
    public string Notes { get; set; } = string.Empty;

    [Required]
    public TransactionType TransactionType { get; set; }

    public Guid? EventId { get; set; }
    public Guid? RedemptionId { get; set; }
}
