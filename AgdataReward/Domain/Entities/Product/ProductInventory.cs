using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities.Product
{
    /// <summary>
    /// Inventory tracking entity for a specific product.
    /// </summary>
    public class ProductInventory
    {
        public Guid Id { get; private set; } // Primary Key
        public Guid ProductId { get; private set; } // Foreign Key to Product (ProductInformation.Id)
        public int StockQuantity { get; private set; } // Current stock level
        public bool IsActive { get; private set; } // Indicates if the inventory record is active

        protected ProductInventory() { } // For EF Core
        public virtual ProductInformation? Product { get; private set; } // Navigation property to ProductInformation

        public ProductInventory(Guid id, Guid productId, int stock)
        {
            Id = id == Guid.Empty ? Guid.NewGuid() : id;
            ProductId = productId != Guid.Empty ? productId : throw new ArgumentException("ProductId is required.");
            if (stock < 0) throw new ArgumentException("Stock cannot be negative.");
            StockQuantity = stock;
            IsActive = true;
        }

        public void IncreaseStock(int qty) // Increase stock by qty
        {
            if (qty <= 0) throw new ArgumentException("qty must be positive.", nameof(qty));
            StockQuantity += qty;
        }

        public void ReduceStock(int qty) // Decrease stock by qty
        {
            if (qty <= 0) throw new ArgumentException("qty must be positive.", nameof(qty));
            if (StockQuantity < qty) throw new InvalidOperationException("Insufficient stock.");
            StockQuantity -= qty;
        }

        public void Deactivate() => IsActive = false; // Soft delete

    }
}