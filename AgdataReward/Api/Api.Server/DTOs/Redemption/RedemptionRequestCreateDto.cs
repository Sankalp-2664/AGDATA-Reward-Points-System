using System.ComponentModel.DataAnnotations;

namespace Api.Server.DTOs.Redemption;

public class RedemptionRequestCreateDto
{
    [Required]
    public Guid RedemptionId { get; set; }

    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "PointsUsed must be positive.")]
    public int PointsUsed { get; set; }
}
