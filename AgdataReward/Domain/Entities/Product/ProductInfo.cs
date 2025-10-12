using Domain.Entities.Reward;
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
    public class ProductInfo
    {
        public Guid Id { get; private set; } // Primary key
        public string SKU { get; private set; } = string.Empty;
        public string Name { get; private set; } = string.Empty;
        public Guid RewardPointsId { get; private set; } // Foreign key to RewardPoints (RewardPoints.Id)
        public virtual RewardPoints? RewardPoints { get; private set; }
        protected ProductInfo() { } // For ORM

        public ProductInfo(Guid id, string sku, string name, Guid rewardPointsId)
        {
            Id = id == Guid.Empty ? Guid.NewGuid() : id;
            SKU = !string.IsNullOrWhiteSpace(sku) ? sku : throw new ArgumentException("SKU is required.");
            Name = !string.IsNullOrWhiteSpace(name) ? name : throw new ArgumentException("Product name is required.");
            RewardPointsId = rewardPointsId != Guid.Empty ? rewardPointsId : throw new ArgumentException("RewardPointsId cannot be empty.");
        }
    }
}