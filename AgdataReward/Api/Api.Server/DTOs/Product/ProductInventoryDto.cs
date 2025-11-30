namespace Api.Server.DTOs.Product;

public class ProductInventoryDto
{
    public Guid Id { get; set; }
    public Guid ProductId { get; set; }
    public int StockQuantity { get; set; }
    public bool IsActive { get; set; }
}
