namespace Api.Server.DTOs.Product;

public class ProductInformationDto
{
    public Guid Id { get; set; }
    public string SKU { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public Guid RewardPointsId { get; set; }
    public int RewardPointsValue { get; set; }
}
