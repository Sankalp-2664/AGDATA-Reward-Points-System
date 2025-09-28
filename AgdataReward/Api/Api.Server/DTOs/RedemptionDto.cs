namespace Api.Server.DTOs
{
    public class RedemptionDto
    {
        public Guid RedemptionId { get; set; }
        public Guid UserId { get; set; }
        public Guid ProductId { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime RequestedAt { get; set; }
        public DateTime? ApprovedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
    }
}
