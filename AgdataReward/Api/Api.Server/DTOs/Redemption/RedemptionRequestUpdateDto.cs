using Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace Api.Server.DTOs.Redemption;

public class RedemptionRequestUpdateDto
{
    [Required]
    public Guid Id { get; set; }

    [Required]
    public RedemptionStatus Status { get; set; }
}
