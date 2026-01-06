using Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace Api.Server.DTOs.Reward;

public class RewardTransactionUpdateDto
{
    [Required]
    public Guid Id { get; set; }

    [StringLength(500)]
    public string? Notes { get; set; }

    public TransactionType? TransactionType { get; set; }
}
