using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IProductService
    {
        Task<ProductInfo> AddProductAsync(string sku, string name, Guid rewardPointsId);
        Task<IEnumerable<ProductInfo>> GetCatalogAsync();
    }
}
