using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces;

public interface IProductRepository
{
    Task<ProductInfo?> GetByIdAsync(Guid id);
    Task<ProductInfo?> GetBySkuAsync(string sku);
    Task AddAsync(ProductInfo product);
    Task<IEnumerable<ProductInfo>> ListAsync();
}
