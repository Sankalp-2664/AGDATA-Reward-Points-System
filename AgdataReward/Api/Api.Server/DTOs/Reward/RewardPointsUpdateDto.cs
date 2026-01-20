using System.ComponentModel.DataAnnotations;

namespace Api.Server.DTOs.Reward;

public class RewardPointsUpdateDto
{
    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "PointsValue must be greater than zero.")]
    public int PointsValue { get; set; }
}
