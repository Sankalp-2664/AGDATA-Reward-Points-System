using Application.Interfaces;
using Domain.Entities.Product;

namespace Application.Services;

public class InventoryService(
    IProductInventoryRepository inventoryRepository) : IInventoryService
{
    private readonly IProductInventoryRepository _inventoryRepository = inventoryRepository;

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
