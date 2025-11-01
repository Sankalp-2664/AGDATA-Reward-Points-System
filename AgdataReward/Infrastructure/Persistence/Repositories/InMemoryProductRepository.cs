using Application.Interfaces;
using Domain.ValueObjects;
using Domain.Entities.Product;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Persistence.Repositories
{
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
    }
}
