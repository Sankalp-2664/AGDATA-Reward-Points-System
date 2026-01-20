using Application.Interfaces;
using Domain.Entities.Product;
using Domain.ValueObjects;

namespace Application.Services;

public class ProductService(
    IProductRepository productRepository,
    IRewardPointsRepository rewardPointsRepository,
    IProductInventoryRepository productInventoryRepository) : IProductService
{
    private readonly IProductRepository _productRepository = productRepository;
    private readonly IRewardPointsRepository _rewardPointsRepository = rewardPointsRepository;
    private readonly IProductInventoryRepository _productInventoryRepository = productInventoryRepository;

    public async Task<ProductInformation> AddProductAsync(string skuString, string name, Guid rewardPointsId)
    {
        var sku = new SKU(skuString);

        var allProducts = await _productRepository.ListAsync();
        var duplicate = allProducts.FirstOrDefault(p =>
            p.SKU.Equals(sku) &&
            string.Equals(p.Name, name, StringComparison.OrdinalIgnoreCase)
        );

        if (duplicate != null)
            throw new InvalidOperationException(
                $"A product with SKU '{skuString}' and Name '{name}' already exists."
            );

        var rewardPoints = await _rewardPointsRepository.GetByIdAsync(rewardPointsId);
        if (rewardPoints == null)
            throw new ArgumentException("Invalid reward points configuration.");

        var product = new ProductInformation(Guid.NewGuid(), sku, name, rewardPointsId);
        await _productRepository.AddAsync(product);

        var inventory = new ProductInventory(
            Guid.NewGuid(),   // Inventory ID
            product.Id,       // ProductId
            0                 // Initial stock
        );

        await _productInventoryRepository.AddAsync(inventory);

        return product;
    }


    public async Task<IEnumerable<ProductInformation>> GetCatalogAsync()
        => await _productRepository.ListAsync();

    public async Task<ProductInformation?> GetByIdAsync(Guid id)
    {
        return await _productRepository.GetByIdAsync(id);
    }

    public async Task<bool> DeleteProductAsync(Guid id)
    {
        var existing = await _productRepository.GetByIdAsync(id);
        if (existing == null) return false;

        await _productRepository.DeleteAsync(id);
        return true;
    }

    public async Task<ProductInformation> UpdateProductAsync(Guid id, string? skuString, string? name, Guid? rewardPointsId)
    {
        var product = await _productRepository.GetByIdAsync(id);
        if (product == null)
            throw new ArgumentException("Product not found.");

        // Update SKU if provided
        if (!string.IsNullOrWhiteSpace(skuString))
        {
            var newSku = new SKU(skuString);
            product.UpdateSKU(newSku);
        }

        // Update Name if provided
        if (!string.IsNullOrWhiteSpace(name))
        {
            product.UpdateName(name);
        }

        // Update RewardPoints if provided
        if (rewardPointsId.HasValue && rewardPointsId.Value != Guid.Empty)
        {
            var rewardPoints = await _rewardPointsRepository.GetByIdAsync(rewardPointsId.Value);
            if (rewardPoints == null)
                throw new ArgumentException("Invalid reward points configuration.");
            
            product.UpdateRewardPoints(rewardPointsId.Value);
        }

        await _productRepository.UpdateAsync(product);
        return await _productRepository.GetByIdAsync(id) ?? product;
    }
}
