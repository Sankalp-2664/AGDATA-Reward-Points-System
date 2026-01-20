using Application.Interfaces;
using Domain.Entities.Product;
using Domain.ValueObjects;

namespace Infrastructure.Persistence.Repositories;

public class InMemoryProductRepository : IProductRepository
{
    private readonly List<ProductInformation> _products = new();

    public Task<ProductInformation?> GetByIdAsync(Guid id)
        => Task.FromResult(_products.FirstOrDefault(p => p.Id == id));

    public Task<ProductInformation?> GetBySkuAsync(SKU sku)
    {
        return Task.FromResult(
            _products.FirstOrDefault(p => p.SKU.Equals(sku))
        );
    }

    public Task AddAsync(ProductInformation product)
    {
        _products.Add(product);
        return Task.CompletedTask;
    }

    public Task<IEnumerable<ProductInformation>> ListAsync()
        => Task.FromResult<IEnumerable<ProductInformation>>(_products);

    public Task DeleteAsync(Guid id)
    {
        var item = _products.FirstOrDefault(p => p.Id == id);
        if (item != null)
            _products.Remove(item);

        return Task.CompletedTask;
    }

    public Task UpdateAsync(ProductInformation product)
    {
        var existing = _products.FirstOrDefault(p => p.Id == product.Id);
        if (existing != null)
        {
            _products.Remove(existing);
            _products.Add(product);
        }
        return Task.CompletedTask;
    }
}
