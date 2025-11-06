using Domain.Entities.Product;
using Domain.Entities.User;

namespace Domain.Entities.Redemption;

/// <summary>
/// Represents a record of a product redeemed by a user.
/// </summary>
public class RedemptionRecord
{
    public Guid Id { get; private set; } // Primary Key
    public Guid UserId { get; private set; } // Foreign Key to UserProfile (UserProfile.Id)
    public Guid ProductId { get; private set; } // Foreign Key to ProductInformation (ProductInformation.Id)
    public DateTime RedeemedAt { get; private set; } // Timestamp when the product was redeemed
    public virtual UserProfile? User { get; private set; } // For navigation between UserProfile and RedemptionRecord
    public virtual ProductInformation? Product { get; private set; } // For navigation between ProductInformation and RedemptionRecord
    protected RedemptionRecord() { } // For ORM

    public RedemptionRecord(Guid id, Guid userId, Guid productId)
    {
        if (id == Guid.Empty) throw new ArgumentException("Id cannot be empty.");
        if (userId == Guid.Empty) throw new ArgumentException("UserId cannot be empty.");
        if (productId == Guid.Empty) throw new ArgumentException("ProductId cannot be empty.");

        Id = id;
        UserId = userId;
        ProductId = productId;
        RedeemedAt = DateTime.UtcNow;
    }
}
