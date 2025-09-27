using Application.Interfaces;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Persistence.Repositories
{
    public class InMemoryProductInventoryRepository : IProductInventoryRepository
    {
        private readonly Dictionary<Guid, ProductInventory> _inventory = new();

        public Task<ProductInventory?> GetByProductIdAsync(Guid productId)
        {
            _inventory.TryGetValue(productId, out var inv);
            return Task.FromResult(inv);
        }

        public Task UpdateAsync(ProductInventory inventory)
        {
            _inventory[inventory.ProductId] = inventory;
            return Task.CompletedTask;
        }
    }
}
