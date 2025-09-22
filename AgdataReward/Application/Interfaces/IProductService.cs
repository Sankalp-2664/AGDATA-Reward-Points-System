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
        Product AddProduct(Product product);
        IEnumerable<Product> GetAllProducts();
        Product? GetById(int id);
        Product UpdateProduct(Product product);
        void DeleteProduct(int id);
    }
}
