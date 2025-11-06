using Domain.Entities.Event;

namespace Api.Server.DTOs.Event;

public class EventDefinitionDto
{
    public Guid Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;

    public List<EventInstanceDto> Instances { get; set; } = new();
    public List<EventRewardRuleDto> RewardRules { get; set; } = new();

    public static EventDefinitionDto FromDomain(EventDefinition entity)
    {
        return new EventDefinitionDto
        {
            Id = entity.Id,
            Code = entity.Code,
            Title = entity.Title,
            Instances = entity.Instances?.Select(EventInstanceDto.FromDomain).ToList() ?? new(),
            RewardRules = entity.RewardRules?.Select(EventRewardRuleDto.FromDomain).ToList() ?? new()
        };
    }
}
