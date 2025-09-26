using Application.Interfaces;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public class InventoryService : IInventoryService
    {
        private readonly IProductInventoryRepository _inventoryRepository;

        public InventoryService(IProductInventoryRepository inventoryRepository)
        {
            _inventoryRepository = inventoryRepository;
        }

        public async Task<ProductInventory?> GetInventoryAsync(Guid productId)
            => await _inventoryRepository.GetByProductIdAsync(productId);

        public async Task UpdateStockAsync(Guid productId, int quantityChange)
        {
            var inventory = await _inventoryRepository.GetByProductIdAsync(productId);
            if (inventory == null)
                throw new ArgumentException("Invalid product ID.");

            inventory.IncreaseStock(quantityChange);
            await _inventoryRepository.UpdateAsync(inventory);
        }
    }
}
