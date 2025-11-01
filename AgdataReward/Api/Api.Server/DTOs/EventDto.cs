namespace Api.Server.DTOs
{
    public class EventDto
    {
        public Guid Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public List<EventRewardRuleDto> RewardRules { get; set; } = new();

        public static EventDto FromDomain(Domain.Entities.Event.EventDefinition entity)
        {
            return new EventDto
            {
                Id = entity.Id,
                Code = entity.Code,
                Title = entity.Title,
                RewardRules = entity.RewardRules?
                    .Select(r => new EventRewardRuleDto
                    {
                        Rank = r.Rank,
                        RewardPointsId = r.RewardPointsId
                    }).ToList() ?? new List<EventRewardRuleDto>()
            };
        }
    }

    public class EventRewardRuleDto
    {
        public int Rank { get; set; }
        public Guid RewardPointsId { get; set; }
    }
}
