using System.ComponentModel.DataAnnotations;

namespace Api.Server.DTOs.Event;

public class EventDefinitionCreateDto
{
    [Required]
    [StringLength(100)]
    public string Code { get; set; } = string.Empty;

    [Required]
    [StringLength(200)]
    public string Title { get; set; } = string.Empty;

    [Required]
    public DateTime StartDate { get; set; }

    [Required]
    public DateTime EndDate { get; set; }

    // Direct prize point values (will auto-create reward points)
    public int? FirstPrize { get; set; }
    public int? SecondPrize { get; set; }
    public int? ThirdPrize { get; set; }
    
    // Alternative: Reward rule IDs for 1st, 2nd, 3rd prizes (optional)
    public Guid? FirstPrizeRewardPointsId { get; set; }
    public Guid? SecondPrizeRewardPointsId { get; set; }
    public Guid? ThirdPrizeRewardPointsId { get; set; }
}
