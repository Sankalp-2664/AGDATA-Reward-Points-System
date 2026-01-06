using Domain.Entities.Product;
using Domain.ValueObjects;

namespace Application.Interfaces;

public interface IProductRepository
{
    Task<ProductInformation?> GetByIdAsync(Guid id);
    Task<ProductInformation?> GetBySkuAsync(SKU sku);
    Task AddAsync(ProductInformation product);
    Task<IEnumerable<ProductInformation>> ListAsync();
    Task DeleteAsync(Guid id);
}
