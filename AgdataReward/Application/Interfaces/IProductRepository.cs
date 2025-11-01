using Domain.Entities.Product;
using Domain.ValueObjects;  
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces;

public interface IProductRepository
{
    Task<ProductInformation?> GetByIdAsync(Guid id);
    Task<ProductInformation?> GetBySkuAsync(SKU sku);
    Task AddAsync(ProductInformation product);
    Task<IEnumerable<ProductInformation>> ListAsync();
}
