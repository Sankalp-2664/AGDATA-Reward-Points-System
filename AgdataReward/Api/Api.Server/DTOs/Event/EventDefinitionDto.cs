namespace Api.Server.DTOs.Event;

public class EventDefinitionDto
{
    public Guid Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string Status { get; set; } = "Upcoming";

    public List<EventInstanceDto> Instances { get; set; } = new();
    public List<EventRewardRuleDto> RewardRules { get; set; } = new();
}
