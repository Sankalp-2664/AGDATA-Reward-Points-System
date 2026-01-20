using Domain.Entities.Product;

namespace Application.Interfaces;

public interface IInventoryService
{
    Task<ProductInventory?> GetInventoryAsync(Guid productId);
    Task UpdateStockAsync(Guid productId, int quantityChange);
    Task UpdateStatusAsync(Guid productId, bool isActive);
}
