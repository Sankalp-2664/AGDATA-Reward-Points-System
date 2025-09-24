using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IInventoryService
    {
        Task<ProductInventory?> GetInventoryAsync(Guid productId);
        Task UpdateStockAsync(Guid productId, int quantityChange);
    }
}
