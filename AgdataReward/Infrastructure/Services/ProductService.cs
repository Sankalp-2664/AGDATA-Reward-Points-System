using System;
using System.Collections.Generic;
using System.Linq;
using Domain.Entities;
using Application.Interfaces;

namespace Infrastructure.Services
{
    public class ProductService : IProductService
    {
        private readonly List<Product> _products = new();
        private int _nextId = 1;

        public Product AddProduct(Product product)
        {
            if (product.PointsRequired <= 0)
                throw new InvalidOperationException("Required points must be positive.");
            if (product.Stock < 0)
                throw new InvalidOperationException("Stock cannot be negative.");

            var newProduct = new Product(_nextId++, product.ProductName, product.PointsRequired, product.Stock);
            _products.Add(newProduct);
            _nextId++;

            return newProduct;
        }

        public IEnumerable<Product> GetAllProducts() => _products;

        public Product? GetById(int id) => _products.FirstOrDefault(p => p.ProductId == id);

        public Product UpdateProduct(Product product)
        {
            var existing = _products.FirstOrDefault(p => p.ProductId == product.ProductId);
            if (existing == null)
                throw new InvalidOperationException("Product not found.");

            _products.Remove(existing);
            _products.Add(product);
            return product;
        }

        public void DeleteProduct(int id)
        {
            var product = _products.FirstOrDefault(p => p.ProductId == id);
            if (product == null)
                throw new InvalidOperationException("Product not found.");
            _products.Remove(product);
        }
    }
}
