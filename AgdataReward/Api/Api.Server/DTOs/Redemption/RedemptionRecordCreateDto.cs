using System.ComponentModel.DataAnnotations;

namespace Api.Server.DTOs.Redemption;

public class RedemptionRecordCreateDto
{
    [Required]
    public Guid UserId { get; set; }

    [Required]
    public Guid ProductId { get; set; }
}
