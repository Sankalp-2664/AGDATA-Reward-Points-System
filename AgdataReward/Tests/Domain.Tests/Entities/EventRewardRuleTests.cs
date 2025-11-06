using Domain.Entities.Event;
using FluentAssertions;

namespace Tests.Domain.Tests.Entities;

public class EventRewardRuleTests
{
    [Fact]
    public void Constructor_Should_Throw_When_Id_Is_Empty()
    {
        // Arrange
        var eventId = Guid.NewGuid();
        var rank = 1;
        var rewardPointsId = Guid.NewGuid();

        // Act
        Action act = () => new EventRewardRule(Guid.Empty, eventId, rank, rewardPointsId);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("Id cannot be empty.*");
    }

    [Fact]
    public void Constructor_Should_Throw_When_EventId_Is_Empty()
    {
        // Arrange
        var id = Guid.NewGuid();
        var rank = 1;
        var rewardPointsId = Guid.NewGuid();

        // Act
        Action act = () => new EventRewardRule(id, Guid.Empty, rank, rewardPointsId);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("EventId cannot be empty.*");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Constructor_Should_Throw_When_Rank_Is_Invalid(int invalidRank)
    {
        // Arrange
        var id = Guid.NewGuid();
        var eventId = Guid.NewGuid();
        var rewardPointsId = Guid.NewGuid();

        // Act
        Action act = () => new EventRewardRule(id, eventId, invalidRank, rewardPointsId);

        // Assert
        act.Should().Throw<ArgumentOutOfRangeException>()
            .WithMessage("Rank must be greater than zero.*");
    }

    [Fact]
    public void Constructor_Should_Throw_When_RewardPointsId_Is_Empty()
    {
        // Arrange
        var id = Guid.NewGuid();
        var eventId = Guid.NewGuid();
        var rank = 1;

        // Act
        Action act = () => new EventRewardRule(id, eventId, rank, Guid.Empty);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("RewardPointsId cannot be empty.*");
    }

    [Fact]
    public void Constructor_Should_Initialize_Correctly()
    {
        // Arrange
        var id = Guid.NewGuid();
        var eventId = Guid.NewGuid();
        var rank = 1;
        var rewardPointsId = Guid.NewGuid();

        // Act
        var rule = new EventRewardRule(id, eventId, rank, rewardPointsId);

        // Assert
        rule.Id.Should().Be(id);
        rule.EventId.Should().Be(eventId);
        rule.Rank.Should().Be(rank);
        rule.RewardPointsId.Should().Be(rewardPointsId);
    }

    [Fact]
    public void UpdateRewardPoints_Should_Throw_When_Id_Is_Empty()
    {
        // Arrange
        var rule = new EventRewardRule(Guid.NewGuid(), Guid.NewGuid(), 1, Guid.NewGuid());

        // Act
        Action act = () => rule.UpdateRewardPoints(Guid.Empty);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("New RewardPointsId cannot be empty.*");
    }

    [Fact]
    public void UpdateRewardPoints_Should_Update_Id()
    {
        // Arrange
        var initialRewardPointsId = Guid.NewGuid();
        var rule = new EventRewardRule(Guid.NewGuid(), Guid.NewGuid(), 1, initialRewardPointsId);
        var newRewardPointsId = Guid.NewGuid();

        // Act
        rule.UpdateRewardPoints(newRewardPointsId);

        // Assert
        rule.RewardPointsId.Should().Be(newRewardPointsId);
    }
}