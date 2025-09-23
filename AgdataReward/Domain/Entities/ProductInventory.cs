using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities;

public class ProductInventory
{
    public Guid ProductId { get; }
    public int StockQuantity { get; private set; }
    public bool IsActive { get; private set; }

    public ProductInventory(Guid productId, int stock)
    {
        ProductId = productId;
        StockQuantity = stock;
        IsActive = true;
    }

    public void ReduceStock(int qty)
    {
        if (qty <= 0) throw new ArgumentException("Quantity must be positive.");
        if (StockQuantity < qty) throw new InvalidOperationException("Not enough stock.");
        StockQuantity -= qty;
    }

    public void IncreaseStock(int qty)
    {
        if (qty <= 0) throw new ArgumentException("Quantity must be positive.");
        StockQuantity += qty;
    }

    public void Deactivate() => IsActive = false;
}

