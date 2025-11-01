using Domain.Entities.Reward;
using Domain.Entities.User;
using Domain.Enums;
using Domain.Exceptions;
using FluentAssertions;
using System;
using Xunit;

namespace Tests.Domain.Tests.Entities
{
    public class UserAccountTests
    {
        private RewardTransaction CreateTransaction(int points, TransactionType type)
        {
            return new RewardTransaction(Guid.NewGuid(), points, "Test transaction", type);
        }

        [Fact]
        public void Constructor_Should_Throw_When_UserId_Is_Empty()
        {
            Action act = () => new UserAccount(Guid.Empty);
            act.Should().Throw<ArgumentException>()
                .WithMessage("*UserId cannot be empty*");
        }

        [Fact]
        public void Constructor_Should_Initialize_Correctly()
        {
            var userId = Guid.NewGuid();
            var account = new UserAccount(userId);

            account.UserId.Should().Be(userId);
            account.RewardBalance.Should().Be(0);
            account.Status.Should().Be(AccountStatus.Active);
            account.Transactions.Should().BeEmpty();
        }

        [Fact]
        public void AddPoints_Should_Throw_When_Transaction_Null()
        {
            var account = new UserAccount(Guid.NewGuid());
            Action act = () => account.AddPoints(10, null!);
            act.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void AddPoints_Should_Throw_When_Points_NonPositive()
        {
            var account = new UserAccount(Guid.NewGuid());
            var tx = CreateTransaction(10, TransactionType.Credit);
            Action act = () => account.AddPoints(0, tx);
            act.Should().Throw<ArgumentException>().WithMessage("*Points must be positive*");
        }

        [Fact]
        public void AddPoints_Should_Throw_When_Transaction_NotCredit()
        {
            var account = new UserAccount(Guid.NewGuid());
            var tx = CreateTransaction(10, TransactionType.Debit);
            Action act = () => account.AddPoints(10, tx);
            act.Should().Throw<InvalidOperationException>().WithMessage("*Transaction must be of type Credit*");
        }

        [Fact]
        public void AddPoints_Should_Add_Points_And_RecordTransaction()
        {
            var account = new UserAccount(Guid.NewGuid());
            var tx = CreateTransaction(100, TransactionType.Credit);

            account.AddPoints(100, tx);

            account.RewardBalance.Should().Be(100);
            account.Transactions.Should().ContainSingle().Which.Should().Be(tx);
        }

        [Fact]
        public void RedeemPoints_Should_Throw_When_Transaction_Null()
        {
            var account = new UserAccount(Guid.NewGuid());
            Action act = () => account.RedeemPoints(10, null!);
            act.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void RedeemPoints_Should_Throw_When_Points_NonPositive()
        {
            var account = new UserAccount(Guid.NewGuid());
            var tx = CreateTransaction(10, TransactionType.Debit);
            Action act = () => account.RedeemPoints(0, tx);
            act.Should().Throw<ArgumentException>().WithMessage("*Points must be positive*");
        }

        [Fact]
        public void RedeemPoints_Should_Throw_When_InsufficientPoints()
        {
            var account = new UserAccount(Guid.NewGuid());
            var tx = CreateTransaction(50, TransactionType.Debit);
            Action act = () => account.RedeemPoints(50, tx);
            act.Should().Throw<InsufficientPointsException>()
                .Where(e => e.CurrentBalance == 0 && e.Attempted == 50);
        }

        [Fact]
        public void RedeemPoints_Should_Throw_When_Transaction_NotDebit()
        {
            var account = new UserAccount(Guid.NewGuid());
            var creditTx = CreateTransaction(100, TransactionType.Credit);
            account.AddPoints(100, creditTx);
            var debitTx = CreateTransaction(50, TransactionType.Credit);

            Action act = () => account.RedeemPoints(50, debitTx);
            act.Should().Throw<InvalidOperationException>().WithMessage("*Transaction must be of type Debit*");
        }

        [Fact]
        public void RedeemPoints_Should_ReduceBalance_And_RecordTransaction()
        {
            var account = new UserAccount(Guid.NewGuid());
            var creditTx = CreateTransaction(100, TransactionType.Credit);
            account.AddPoints(100, creditTx);

            var debitTx = CreateTransaction(50, TransactionType.Debit);
            account.RedeemPoints(50, debitTx);

            account.RewardBalance.Should().Be(50);
            account.Transactions.Should().HaveCount(2);
            account.Transactions.Should().Contain(debitTx);
        }

        [Fact]
        public void SuspendAccount_Should_Change_Status()
        {
            var account = new UserAccount(Guid.NewGuid());
            account.SuspendAccount();
            account.Status.Should().Be(AccountStatus.Inactive);
        }

        [Fact]
        public void SuspendAccount_Should_Throw_If_AlreadyInactive()
        {
            var account = new UserAccount(Guid.NewGuid());
            account.SuspendAccount();
            Action act = () => account.SuspendAccount();
            act.Should().Throw<InvalidOperationException>().WithMessage("*already inactive*");
        }

        [Fact]
        public void ActivateAccount_Should_Change_Status()
        {
            var account = new UserAccount(Guid.NewGuid());
            account.SuspendAccount();
            account.ActivateAccount();
            account.Status.Should().Be(AccountStatus.Active);
        }

        [Fact]
        public void ActivateAccount_Should_Throw_If_AlreadyActive()
        {
            var account = new UserAccount(Guid.NewGuid());
            Action act = () => account.ActivateAccount();
            act.Should().Throw<InvalidOperationException>().WithMessage("*already active*");
        }
    }
}
