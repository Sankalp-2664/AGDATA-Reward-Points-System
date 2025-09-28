namespace Api.Server.DTOs
{
    public class ProductDto
    {
        public Guid ProductId { get; set; }
        public string Sku { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public Guid RewardPointsId { get; set; }
        public int Stock { get; set; }
    }
}
