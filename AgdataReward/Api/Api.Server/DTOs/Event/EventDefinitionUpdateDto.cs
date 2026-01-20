using System.ComponentModel.DataAnnotations;

namespace Api.Server.DTOs.Event;

public class EventDefinitionUpdateDto
{
    [Required]
    public Guid Id { get; set; }

    [StringLength(100)]
    public string? Code { get; set; }

    [StringLength(200)]
    public string? Title { get; set; }
    
    public string? Name { get; set; } // Alias for Title (from frontend)
    
    public string? Description { get; set; } // Optional description

    public DateTime? StartDate { get; set; }

    public DateTime? EndDate { get; set; }
    
    /// <summary>
    /// Event status: Upcoming, Active, Completed, Cancelled
    /// </summary>
    public string? Status { get; set; }

    // Direct prize point values (will auto-create/update reward points)
    public int? FirstPrize { get; set; }
    public int? SecondPrize { get; set; }
    public int? ThirdPrize { get; set; }
    
    // Alternative: Reward rule IDs for 1st, 2nd, 3rd prizes (optional)
    public Guid? FirstPrizeRewardPointsId { get; set; }
    public Guid? SecondPrizeRewardPointsId { get; set; }
    public Guid? ThirdPrizeRewardPointsId { get; set; }
}
