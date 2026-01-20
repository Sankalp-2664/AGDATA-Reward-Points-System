namespace Api.Server.DTOs.Event;

public class EventWithRewardsDto
{
    public Guid Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string Status { get; set; } = "Upcoming";
    public int ParticipantsCount { get; set; }
    public bool WinnersAssigned { get; set; }
    public bool IsParticipated { get; set; }
    public List<RewardRuleWithPointsDto> RewardRules { get; set; } = new();
}

public class RewardRuleWithPointsDto
{
    public Guid Id { get; set; }
    public int Rank { get; set; }
    public Guid RewardPointsId { get; set; }
    public int PointsValue { get; set; }
}
