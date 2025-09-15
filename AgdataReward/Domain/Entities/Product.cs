using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Product
    {
        public int ProductId { get; private set; }
        public string ProductName { get; private set; }
        public int PointsRequired { get; private set; }
        public int Stock { get; private set; }

        public Product(int productId, string productName, int pointsRequired, int stock)
        {
            ProductId = productId;
            ProductName = productName ?? throw new ArgumentNullException(nameof(productName));
            PointsRequired = pointsRequired;
            Stock = stock;
        }

        public void DeductStock()
        {
            if (Stock <= 0) throw new InvalidOperationException("Out of stock");
            Stock--;
        }

        public void IncreaseStock(int amount)
        {
            if (amount <= 0) throw new ArgumentException("Amount must be positive");
            Stock += amount;
        }

        public bool CheckAvailability() => Stock > 0;
    }
}
