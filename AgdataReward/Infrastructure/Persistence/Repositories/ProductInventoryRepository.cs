using Application.Interfaces;
using Domain.Entities.Product;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public class ProductInventoryRepository : IProductInventoryRepository
{
    private readonly RewardDbContext _context;

    public ProductInventoryRepository(RewardDbContext context)
    {
        _context = context;
    }

    public async Task<ProductInventory?> GetByProductIdAsync(Guid productId)
    {
        return await _context.ProductInventories
            .Include(pi => pi.Product)
            .FirstOrDefaultAsync(pi => pi.ProductId == productId);
    }

    public async Task AddAsync(ProductInventory inventory)
    {
        await _context.ProductInventories.AddAsync(inventory);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(ProductInventory inventory)
    {
        if (_context.Entry(inventory).State == EntityState.Detached)
            _context.ProductInventories.Update(inventory);

        await _context.SaveChangesAsync();
    }
}
