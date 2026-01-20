using Application.Interfaces;
using Domain.Entities.Product;
using Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public class ProductRepository(RewardDbContext context) : IProductRepository
{
    private readonly RewardDbContext _context = context;

    public async Task<ProductInformation?> GetByIdAsync(Guid id)
    {
        return await _context.ProductInformations
            .Include(p => p.RewardPoints)
            .SingleOrDefaultAsync(p => p.Id == id);
    }

    public async Task<ProductInformation?> GetBySkuAsync(SKU sku)
    {
        return await _context.ProductInformations
            .Include(p => p.RewardPoints)
            .SingleOrDefaultAsync(p => p.SKU == sku);
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

    public async Task DeleteAsync(Guid id)
    {
        var entity = await _context.ProductInformations.FindAsync(id);
        if (entity != null)
        {
            _context.ProductInformations.Remove(entity);
            await _context.SaveChangesAsync();
        }
    }

    public async Task UpdateAsync(ProductInformation product)
    {
        _context.ProductInformations.Update(product);
        await _context.SaveChangesAsync();
    }
}
