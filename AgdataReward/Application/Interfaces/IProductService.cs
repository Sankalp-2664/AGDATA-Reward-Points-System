using Domain.Entities.Product;

namespace Application.Interfaces;

public interface IProductService
{
    Task<ProductInformation> AddProductAsync(string sku, string name, Guid rewardPointsId);
    Task<IEnumerable<ProductInformation>> GetCatalogAsync();
}
