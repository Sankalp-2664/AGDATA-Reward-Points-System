namespace Api.Server.DTOs
{
    public class EventDto
    {
        public Guid EventId { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;

        public List<EventRewardRuleDto> RewardRules { get; set; } = new();
    }

    public class EventRewardRuleDto
    {
        public int Rank { get; set; }
        public Guid RewardPointsId { get; set; }
    }

}
