namespace Api.Server.DTOs
{
    public class TransactionDto
    {
        public Guid TransactionId { get; set; }
        public Guid UserId { get; set; }
        public int PointsDelta { get; set; }
        public string Notes { get; set; } = string.Empty;
        public Guid? EventId { get; set; }
        public Guid? RedemptionId { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
