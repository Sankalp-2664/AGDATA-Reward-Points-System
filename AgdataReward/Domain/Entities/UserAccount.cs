using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Enums;

namespace Domain.Entities;

public class UserAccount
{
    public Guid UserId { get; }
    public int RewardBalance { get; private set; }
    public AccountStatus Status { get; private set; }

    private readonly List<RewardTransaction> _transactions = new();
    public IReadOnlyCollection<RewardTransaction> Transactions => _transactions.AsReadOnly();

    public UserAccount(Guid userId)
    {
        UserId = userId;
        RewardBalance = 0;
        Status = AccountStatus.Active;
    }

    public void AddPoints(int points, RewardTransaction tx)
    {
        if (points <= 0) throw new ArgumentException("Points must be positive.");
        RewardBalance += points;
        _transactions.Add(tx);
    }

    public void RedeemPoints(int points, RewardTransaction tx)
    {
        if (points <= 0) throw new ArgumentException("Points must be positive.");
        if (RewardBalance < points) throw new InvalidOperationException("Insufficient balance.");
        RewardBalance -= points;
        _transactions.Add(tx);
    }
}

