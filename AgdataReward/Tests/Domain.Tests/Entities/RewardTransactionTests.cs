using Domain.Entities.Reward;
using Domain.Enums;
using FluentAssertions;

namespace Tests.Domain.Tests.Entities;

public class RewardTransactionTests
{
    [Fact]
    public void Constructor_Should_Throw_When_UserId_Is_Empty()
    {
        // Arrange
        var points = 10;
        var notes = "Test transaction";
        var type = TransactionType.Credit;

        // Act
        Action act = () => new RewardTransaction(Guid.Empty, points, notes, type);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("UserId cannot be empty.*");
    }

    [Fact]
    public void Constructor_Should_Throw_When_PointsDelta_Is_Zero()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var notes = "Test transaction";
        var type = TransactionType.Credit;

        // Act
        Action act = () => new RewardTransaction(userId, 0, notes, type);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("PointsDelta cannot be zero.*");
    }

    [Fact]
    public void Constructor_Should_Throw_When_Notes_Is_NullOrWhitespace()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var points = 10;
        var type = TransactionType.Credit;

        // Act
        Action act1 = () => new RewardTransaction(userId, points, null!, type);
        Action act2 = () => new RewardTransaction(userId, points, "", type);
        Action act3 = () => new RewardTransaction(userId, points, "  ", type);

        // Assert
        act1.Should().Throw<ArgumentException>().WithMessage("Notes cannot be empty.*");
        act2.Should().Throw<ArgumentException>().WithMessage("Notes cannot be empty.*");
        act3.Should().Throw<ArgumentException>().WithMessage("Notes cannot be empty.*");
    }

    [Fact]
    public void Constructor_Should_Initialize_All_Properties_Correctly()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var pointsDelta = 50;
        var notes = "Reward for event";
        var transactionType = TransactionType.Credit;
        var eventId = Guid.NewGuid();
        var redemptionId = Guid.NewGuid();

        // Act
        var transaction = new RewardTransaction(userId, pointsDelta, notes, transactionType, eventId, redemptionId);

        // Assert
        transaction.Id.Should().NotBeEmpty();
        transaction.UserId.Should().Be(userId);
        transaction.PointsDelta.Should().Be(pointsDelta);
        transaction.Notes.Should().Be(notes);
        transaction.TransactionType.Should().Be(transactionType);
        transaction.EventId.Should().Be(eventId);
        transaction.RedemptionId.Should().Be(redemptionId);
        transaction.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));

        transaction.UserAccount.Should().BeNull();
        transaction.EventInstance.Should().BeNull();
        transaction.RedemptionRequest.Should().BeNull();
    }

    [Fact]
    public void Constructor_Should_Allow_Optional_EventId_And_RedemptionId_To_Be_Null()
    {
        // Arrange
        var userId = Guid.NewGuid();

        // Act
        var transaction = new RewardTransaction(userId, 20, "Some notes", TransactionType.Debit);

        // Assert
        transaction.EventId.Should().BeNull();
        transaction.RedemptionId.Should().BeNull();
    }
}