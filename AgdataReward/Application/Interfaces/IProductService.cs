using Domain.Entities.Product;
using Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IProductService
    {
        Task<ProductInformation> AddProductAsync(string sku, string name, Guid rewardPointsId);
        Task<IEnumerable<ProductInformation>> GetCatalogAsync();
    }
}
