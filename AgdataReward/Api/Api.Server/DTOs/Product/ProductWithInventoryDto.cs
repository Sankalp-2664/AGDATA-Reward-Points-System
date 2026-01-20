namespace Api.Server.DTOs.Product;

/// <summary>
/// Combined DTO that includes product information along with inventory details.
/// This is used by the frontend admin dashboard for product management.
/// </summary>
public class ProductWithInventoryDto
{
    public Guid Id { get; set; }
    public string SKU { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public Guid RewardPointsId { get; set; }
    public int RewardPointsValue { get; set; }
    public int Stock { get; set; }
    public bool IsActive { get; set; }
}
