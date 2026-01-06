using Domain.Entities.Redemption;
using Domain.Enums;
using FluentAssertions;

namespace Tests.Domain.Tests.Entities;

public class RedemptionRequestTests
{
    [Fact]
    public void Constructor_Should_Throw_When_PointsUsed_Is_NonPositive()
    {
        // Arrange
        var id1 = Guid.NewGuid();
        var id2 = Guid.NewGuid();

        // Act
        Action act1 = () => new RedemptionRequest(id1, 0);
        Action act2 = () => new RedemptionRequest(id2, -10);

        // Assert
        act1.Should().Throw<ArgumentException>().WithMessage("*PointsUsed must be positive*");
        act2.Should().Throw<ArgumentException>().WithMessage("*PointsUsed must be positive*");
    }

    [Fact]
    public void Constructor_Should_Initialize_Values_Correctly()
    {
        // Arrange
        var redemptionId = Guid.NewGuid();
        int pointsUsed = 100;

        // Act
        var request = new RedemptionRequest(redemptionId, pointsUsed);

        // Assert
        request.RedemptionId.Should().Be(redemptionId);
        request.PointsUsed.Should().Be(pointsUsed);
        request.Status.Should().Be(RedemptionStatus.Pending);
        request.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        request.Id.Should().NotBeEmpty();
    }

    [Fact]
    public void Approve_Should_Set_Status_To_Approved()
    {
        // Arrange
        var request = new RedemptionRequest(Guid.NewGuid(), 50);

        // Act
        request.Approve();

        // Assert
        request.Status.Should().Be(RedemptionStatus.Approved);
    }

    [Fact]
    public void Reject_Should_Set_Status_To_Rejected()
    {
        // Arrange
        var request = new RedemptionRequest(Guid.NewGuid(), 50);

        // Act
        request.Reject();

        // Assert
        request.Status.Should().Be(RedemptionStatus.Rejected);
    }

    [Fact]
    public void MarkCompleted_Should_Set_Status_To_Completed_When_Approved()
    {
        // Arrange
        var request = new RedemptionRequest(Guid.NewGuid(), 50);
        request.Approve();

        // Act
        request.MarkCompleted();

        // Assert
        request.Status.Should().Be(RedemptionStatus.Completed);
    }

    [Fact]
    public void Approve_Should_Throw_When_Status_Is_Not_Pending()
    {
        // Arrange
        var request = new RedemptionRequest(Guid.NewGuid(), 50);
        request.Reject();

        // Act
        Action act = () => request.Approve();

        // Assert
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("Only Pending redemptions can be approved.");
    }

    [Fact]
    public void Reject_Should_Throw_When_Status_Is_Not_Pending()
    {
        // Arrange
        var request = new RedemptionRequest(Guid.NewGuid(), 50);
        request.Approve();

        // Act
        Action act = () => request.Reject();

        // Assert
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("Only Pending redemptions can be rejected.");
    }

    [Fact]
    public void MarkCompleted_Should_Throw_When_Status_Is_Not_Approved()
    {
        // Arrange - pending case
        var requestPending = new RedemptionRequest(Guid.NewGuid(), 50);

        // Act & Assert - pending -> completed not allowed
        Action actPending = () => requestPending.MarkCompleted();
        actPending.Should().Throw<InvalidOperationException>()
            .WithMessage("Only Approved redemptions can be completed.");

        // Arrange - rejected case
        var requestRejected = new RedemptionRequest(Guid.NewGuid(), 50);
        requestRejected.Reject();

        // Act & Assert - rejected -> completed not allowed
        Action actRejected = () => requestRejected.MarkCompleted();
        actRejected.Should().Throw<InvalidOperationException>()
            .WithMessage("Only Approved redemptions can be completed.");
    }

    [Fact]
    public void GetAllowedTransitions_Should_Return_Correct_Statuses()
    {
        // Arrange
        var request = new RedemptionRequest(Guid.NewGuid(), 50);

        // Act / Assert - Pending
        request.GetAllowedTransitions().Should().BeEquivalentTo(new[] { RedemptionStatus.Approved, RedemptionStatus.Rejected });

        // Act - Approved
        request.Approve();

        // Assert - Approved
        request.GetAllowedTransitions().Should().BeEquivalentTo(new[] { RedemptionStatus.Completed });

        // Act - Completed
        request.MarkCompleted();

        // Assert - Completed
        request.GetAllowedTransitions().Should().BeEmpty();

        // Arrange / Act / Assert - Rejected
        var rejectedRequest = new RedemptionRequest(Guid.NewGuid(), 50);
        rejectedRequest.Reject();
        rejectedRequest.GetAllowedTransitions().Should().BeEmpty();
    }
}