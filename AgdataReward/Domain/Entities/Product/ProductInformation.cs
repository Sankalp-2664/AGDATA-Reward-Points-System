using Domain.Entities.Reward;
using Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities.Product
{
    /// <summary>
    /// Aggregate root representing the core product definition.
    /// </summary>
    public class ProductInformation
    {
        public Guid Id { get; private set; } // Primary key
        public SKU SKU { get; private set; } = null!; // Stock Keeping Unit
        public string Name { get; private set; } = string.Empty; // Product name
        public Guid RewardPointsId { get; private set; } // Foreign key to RewardPoints (RewardPoints.Id)
        public virtual RewardPoints? RewardPoints { get; private set; } // Navigation property to RewardPoints
        protected ProductInformation() { } // For ORM

        public ProductInformation(Guid id, SKU sku, string name, Guid rewardPointsId)
        {
            Id = id == Guid.Empty ? Guid.NewGuid() : id;
            SKU = sku ?? throw new ArgumentNullException(nameof(sku), "SKU is required.");
            Name = !string.IsNullOrWhiteSpace(name) ? name : throw new ArgumentException("Product name is required.");
            RewardPointsId = rewardPointsId != Guid.Empty ? rewardPointsId : throw new ArgumentException("RewardPointsId cannot be empty.");
        }
    }
}