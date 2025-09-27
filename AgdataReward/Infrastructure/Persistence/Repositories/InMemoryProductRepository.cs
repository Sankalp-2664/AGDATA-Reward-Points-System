using Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Persistence.Repositories
{
    public class InMemoryProductRepository : IProductRepository
    {
        private readonly List<ProductInfo> _products = new();

        public Task<ProductInfo?> GetByIdAsync(Guid id)
            => Task.FromResult(_products.FirstOrDefault(p => p.Id == id));

        public Task<ProductInfo?> GetBySkuAsync(string sku)
            => Task.FromResult(_products.FirstOrDefault(p => p.SKU.Equals(sku, StringComparison.OrdinalIgnoreCase)));

        public Task AddAsync(ProductInfo product)
        {
            _products.Add(product);
            return Task.CompletedTask;
        }

        public Task<IEnumerable<ProductInfo>> ListAsync()
            => Task.FromResult<IEnumerable<ProductInfo>>(_products);
    }
}
