using Domain.Entities.Product;

namespace Application.Interfaces;

public interface IProductInventoryRepository
{
    Task<ProductInventory?> GetByProductIdAsync(Guid productId);
    Task UpdateAsync(ProductInventory inventory);
}

