using Application.Interfaces;
using Domain.Entities.Product;
using Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly RewardDbContext _context;

    public ProductRepository(RewardDbContext context)
    {
        _context = context;
    }

    public async Task<ProductInformation?> GetByIdAsync(Guid id)
    {
        return await _context.ProductInformations
            .Include(p => p.RewardPoints)
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<ProductInformation?> GetBySkuAsync(SKU sku)
    {
        return await _context.ProductInformations
            .Include(p => p.RewardPoints)
            .FirstOrDefaultAsync(p => p.SKU == sku);
    }

    public async Task AddAsync(ProductInformation product)
    {
        await _context.ProductInformations.AddAsync(product);
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<ProductInformation>> ListAsync()
    {
        return await _context.ProductInformations
            .Include(p => p.RewardPoints)
            .ToListAsync();
    }
}
