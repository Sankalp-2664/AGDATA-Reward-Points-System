using Domain.Entities.Reward;
using Domain.Entities.User;
using Domain.Enums;
using Domain.Exceptions;
using FluentAssertions;

namespace Tests.Domain.Tests.Entities;

public class UserAccountTests
{
    private RewardTransaction CreateTransaction(int points, TransactionType type)
    {
        return new RewardTransaction(Guid.NewGuid(), points, "Test transaction", type);
    }

    [Fact]
    public void Constructor_Should_Throw_When_UserId_Is_Empty()
    {
        // Arrange
        // (no setup required)

        // Act
        Action act = () => new UserAccount(Guid.Empty);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*UserId cannot be empty*");
    }

    [Fact]
    public void Constructor_Should_Initialize_Correctly()
    {
        // Arrange
        var userId = Guid.NewGuid();

        // Act
        var account = new UserAccount(userId);

        // Assert
        account.UserId.Should().Be(userId);
        account.RewardBalance.Should().Be(0);
        account.Status.Should().Be(AccountStatus.Active);
        account.Transactions.Should().BeEmpty();
    }

    [Fact]
    public void AddPoints_Should_Throw_When_Transaction_Null()
    {
        // Arrange
        var account = new UserAccount(Guid.NewGuid());

        // Act
        Action act = () => account.AddPoints(10, null!);

        // Assert
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void AddPoints_Should_Throw_When_Points_NonPositive()
    {
        // Arrange
        var account = new UserAccount(Guid.NewGuid());
        var tx = CreateTransaction(10, TransactionType.Credit);

        // Act
        Action act = () => account.AddPoints(0, tx);

        // Assert
        act.Should().Throw<ArgumentException>().WithMessage("*Points must be positive*");
    }

    [Fact]
    public void AddPoints_Should_Throw_When_Transaction_NotCredit()
    {
        // Arrange
        var account = new UserAccount(Guid.NewGuid());
        var tx = CreateTransaction(10, TransactionType.Debit);

        // Act
        Action act = () => account.AddPoints(10, tx);

        // Assert
        act.Should().Throw<InvalidOperationException>().WithMessage("*Transaction must be of type Credit*");
    }

    [Fact]
    public void AddPoints_Should_Add_Points_And_RecordTransaction()
    {
        // Arrange
        var account = new UserAccount(Guid.NewGuid());
        var tx = CreateTransaction(100, TransactionType.Credit);

        // Act
        account.AddPoints(100, tx);

        // Assert
        account.RewardBalance.Should().Be(100);
        account.Transactions.Should().ContainSingle().Which.Should().Be(tx);
    }

    [Fact]
    public void RedeemPoints_Should_Throw_When_Transaction_Null()
    {
        // Arrange
        var account = new UserAccount(Guid.NewGuid());

        // Act
        Action act = () => account.RedeemPoints(10, null!);

        // Assert
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void RedeemPoints_Should_Throw_When_Points_NonPositive()
    {
        // Arrange
        var account = new UserAccount(Guid.NewGuid());
        var tx = CreateTransaction(10, TransactionType.Debit);

        // Act
        Action act = () => account.RedeemPoints(0, tx);

        // Assert
        act.Should().Throw<ArgumentException>().WithMessage("*Points must be positive*");
    }

    [Fact]
    public void RedeemPoints_Should_Throw_When_InsufficientPoints()
    {
        // Arrange
        var account = new UserAccount(Guid.NewGuid());
        var tx = CreateTransaction(50, TransactionType.Debit);

        // Act
        Action act = () => account.RedeemPoints(50, tx);

        // Assert
        act.Should().Throw<InsufficientPointsException>()
            .Where(e => e.CurrentBalance == 0 && e.Attempted == 50);
    }

    [Fact]
    public void RedeemPoints_Should_Throw_When_Transaction_NotDebit()
    {
        // Arrange
        var account = new UserAccount(Guid.NewGuid());
        var creditTx = CreateTransaction(100, TransactionType.Credit);
        account.AddPoints(100, creditTx);
        var debitTx = CreateTransaction(50, TransactionType.Credit);

        // Act
        Action act = () => account.RedeemPoints(50, debitTx);

        // Assert
        act.Should().Throw<InvalidOperationException>().WithMessage("*Transaction must be of type Debit*");
    }

    [Fact]
    public void RedeemPoints_Should_ReduceBalance_And_RecordTransaction()
    {
        // Arrange
        var account = new UserAccount(Guid.NewGuid());
        var creditTx = CreateTransaction(100, TransactionType.Credit);
        account.AddPoints(100, creditTx);
        var debitTx = CreateTransaction(50, TransactionType.Debit);

        // Act
        account.RedeemPoints(50, debitTx);

        // Assert
        account.RewardBalance.Should().Be(50);
        account.Transactions.Should().HaveCount(2);
        account.Transactions.Should().Contain(debitTx);
    }

    [Fact]
    public void SuspendAccount_Should_Change_Status()
    {
        // Arrange
        var account = new UserAccount(Guid.NewGuid());

        // Act
        account.SuspendAccount();

        // Assert
        account.Status.Should().Be(AccountStatus.Inactive);
    }

    [Fact]
    public void SuspendAccount_Should_Throw_If_AlreadyInactive()
    {
        // Arrange
        var account = new UserAccount(Guid.NewGuid());
        account.SuspendAccount();

        // Act
        Action act = () => account.SuspendAccount();

        // Assert
        act.Should().Throw<InvalidOperationException>().WithMessage("*already inactive*");
    }

    [Fact]
    public void ActivateAccount_Should_Change_Status()
    {
        // Arrange
        var account = new UserAccount(Guid.NewGuid());
        account.SuspendAccount();

        // Act
        account.ActivateAccount();

        // Assert
        account.Status.Should().Be(AccountStatus.Active);
    }

    [Fact]
    public void ActivateAccount_Should_Throw_If_AlreadyActive()
    {
        // Arrange
        var account = new UserAccount(Guid.NewGuid());

        // Act
        Action act = () => account.ActivateAccount();

        // Assert
        act.Should().Throw<InvalidOperationException>().WithMessage("*already active*");
    }
}