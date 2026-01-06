using Domain.Entities.Reward;
using Domain.Enums;
using Domain.Exceptions;

namespace Domain.Entities.User;

/// <summary>
/// Represents a user's reward account and its transactions.
/// </summary>
public class UserAccount
{
    public Guid Id { get; private set; } // Primary key for the account
    public Guid UserId { get; } // Foreign key linking to the UserProfile
    public int RewardBalance { get; private set; } // Current reward points balance
    public AccountStatus Status { get; private set; } // Account status (Active, Inactive)
    public virtual UserProfile? User { get; private set; } //// For navigation between UserProfile and UserAccount

    // Credentials (for authentication)
    public string PasswordHash { get; private set; } = string.Empty;
    public string PasswordSalt { get; private set; } = string.Empty;

    private readonly List<RewardTransaction> _transactions = new(); // Backing field for transactions
    public IReadOnlyCollection<RewardTransaction> Transactions => _transactions.AsReadOnly(); // Expose as read-only

    protected UserAccount() { } // For ORM

    public UserAccount(Guid userId)
    {
        if (userId == Guid.Empty) throw new ArgumentException("UserId cannot be empty.");

        Id = Guid.NewGuid();
        UserId = userId;
        RewardBalance = 0;
        Status = AccountStatus.Active;
    }

    // Set credentials (used when registering or resetting password)
    public void SetCredentials(string passwordHash, string passwordSalt)
    {
        if (string.IsNullOrWhiteSpace(passwordHash))
            throw new ArgumentException("Password hash is required.", nameof(passwordHash));
        if (string.IsNullOrWhiteSpace(passwordSalt))
            throw new ArgumentException("Password salt is required.", nameof(passwordSalt));

        PasswordHash = passwordHash;
        PasswordSalt = passwordSalt;
    }

    public void AddPoints(int points, RewardTransaction transaction) // Add points to the account
    {
        if (transaction == null) throw new ArgumentNullException(nameof(transaction));
        if (points <= 0) throw new ArgumentException("Points must be positive.", nameof(points));
        if (transaction.TransactionType != TransactionType.Credit)
            throw new InvalidOperationException("Transaction must be of type Credit.");

        RewardBalance += points;
        _transactions.Add(transaction);
    }

    public void RedeemPoints(int points, RewardTransaction transaction) // Redeem points from the account
    {
        if (transaction == null) throw new ArgumentNullException(nameof(transaction));
        if (points <= 0) throw new ArgumentException("Points must be positive.", nameof(points));
        if (points > RewardBalance)
            throw new InsufficientPointsException(RewardBalance, points);
        if (transaction.TransactionType != TransactionType.Debit)
            throw new InvalidOperationException("Transaction must be of type Debit.");

        RewardBalance -= points;
        _transactions.Add(transaction);
    }

    public void SuspendAccount() // Suspend the account
    {
        if (Status == AccountStatus.Inactive)
            throw new InvalidOperationException("Account is already inactive.");

        Status = AccountStatus.Inactive;
    }
    public void ActivateAccount() // Activate the account
    {
        if (Status == AccountStatus.Active)
            throw new InvalidOperationException("Account is already active.");

        Status = AccountStatus.Active;
    }
}
