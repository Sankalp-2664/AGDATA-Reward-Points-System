using Domain.Entities.Product;

namespace Api.Server.DTOs.Product;

public class ProductInventoryDto
{
    public Guid Id { get; set; }
    public Guid ProductId { get; set; }
    public int StockQuantity { get; set; }
    public bool IsActive { get; set; }

    public static ProductInventoryDto FromDomain(ProductInventory entity)
    {
        return new ProductInventoryDto
        {
            Id = entity.Id,
            ProductId = entity.ProductId,
            StockQuantity = entity.StockQuantity,
            IsActive = entity.IsActive
        };
    }
}
