using Domain.Entities.Reward;
using FluentAssertions;

namespace Tests.Domain.Entities.Reward;

public class RewardPointsTests
{
    [Fact]
    public void Constructor_Should_SetProperties_When_ValidArguments()
    {
        // Arrange
        var id = Guid.NewGuid();
        int pointsValue = 100;

        // Act
        var rewardPoints = new RewardPoints(id, pointsValue);

        // Assert
        rewardPoints.Id.Should().Be(id);
        rewardPoints.PointsValue.Should().Be(pointsValue);
    }

    [Fact]
    public void Constructor_Should_ThrowArgumentException_When_IdIsEmpty()
    {
        // Arrange
        var emptyId = Guid.Empty;
        int pointsValue = 50;

        // Act
        Action act = () => new RewardPoints(emptyId, pointsValue);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("Id cannot be empty*")
            .And.ParamName.Should().Be("id");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-10)]
    public void Constructor_Should_ThrowArgumentOutOfRangeException_When_PointsValueIsInvalid(int invalidPoints)
    {
        // Arrange
        var id = Guid.NewGuid();

        // Act
        Action act = () => new RewardPoints(id, invalidPoints);

        // Assert
        act.Should().Throw<ArgumentOutOfRangeException>()
            .WithMessage("Points must be greater than zero*")
            .And.ParamName.Should().Be("pointsValue");
    }
}
