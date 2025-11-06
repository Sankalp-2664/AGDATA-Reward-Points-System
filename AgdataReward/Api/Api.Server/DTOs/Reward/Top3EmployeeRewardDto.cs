namespace Api.Server.DTOs.Reward;

public class Top3EmployeeRewardDto
{
    public Guid UserId { get; set; }           
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public decimal RewardBalance { get; set; } // Matches ua.RewardBalance
    public int TotalPointsEarned { get; set; } // SUM of rt.PointsDelta
}
