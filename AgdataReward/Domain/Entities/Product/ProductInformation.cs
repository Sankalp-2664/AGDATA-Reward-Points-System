using Domain.Entities.Reward;
using Domain.ValueObjects;

namespace Domain.Entities.Product;

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

    public void UpdateName(string name)
    {
        Name = !string.IsNullOrWhiteSpace(name) ? name : throw new ArgumentException("Product name is required.");
    }

    public void UpdateSKU(SKU sku)
    {
        SKU = sku ?? throw new ArgumentNullException(nameof(sku), "SKU is required.");
    }

    public void UpdateRewardPoints(Guid rewardPointsId)
    {
        RewardPointsId = rewardPointsId != Guid.Empty ? rewardPointsId : throw new ArgumentException("RewardPointsId cannot be empty.");
    }
}