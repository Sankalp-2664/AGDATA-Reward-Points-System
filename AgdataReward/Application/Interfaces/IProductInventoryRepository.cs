using Domain.Entities.Product;

namespace Application.Interfaces;

public interface IProductInventoryRepository
{
    Task<ProductInventory?> GetByProductIdAsync(Guid productId);
    Task AddAsync(ProductInventory inventory);
    Task UpdateAsync(ProductInventory inventory);
}

