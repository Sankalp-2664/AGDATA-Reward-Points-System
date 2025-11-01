using Domain.Entities.Redemption;
using Domain.Enums;
using FluentAssertions;
using System;
using Xunit;

namespace Tests.Domain.Tests.Entities
{
    public class RedemptionRequestTests
    {
        [Fact]
        public void Constructor_Should_Throw_When_PointsUsed_Is_NonPositive()
        {
            Action act1 = () => new RedemptionRequest(Guid.NewGuid(), 0);
            Action act2 = () => new RedemptionRequest(Guid.NewGuid(), -10);

            act1.Should().Throw<ArgumentException>().WithMessage("*PointsUsed must be positive*");
            act2.Should().Throw<ArgumentException>().WithMessage("*PointsUsed must be positive*");
        }

        [Fact]
        public void Constructor_Should_Initialize_Values_Correctly()
        {
            var redemptionId = Guid.NewGuid();
            int pointsUsed = 100;
            var request = new RedemptionRequest(redemptionId, pointsUsed);

            request.RedemptionId.Should().Be(redemptionId);
            request.PointsUsed.Should().Be(pointsUsed);
            request.Status.Should().Be(RedemptionStatus.Pending);
            request.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
            request.Id.Should().NotBeEmpty();
        }

        [Fact]
        public void Approve_Should_Set_Status_To_Approved()
        {
            var request = new RedemptionRequest(Guid.NewGuid(), 50);
            request.Approve();
            request.Status.Should().Be(RedemptionStatus.Approved);
        }

        [Fact]
        public void Reject_Should_Set_Status_To_Rejected()
        {
            var request = new RedemptionRequest(Guid.NewGuid(), 50);
            request.Reject();
            request.Status.Should().Be(RedemptionStatus.Rejected);
        }

        [Fact]
        public void MarkCompleted_Should_Set_Status_To_Completed_When_Approved()
        {
            var request = new RedemptionRequest(Guid.NewGuid(), 50);
            request.Approve();
            request.MarkCompleted();
            request.Status.Should().Be(RedemptionStatus.Completed);
        }

        [Fact]
        public void Approve_Should_Throw_When_Status_Is_Not_Pending()
        {
            var request = new RedemptionRequest(Guid.NewGuid(), 50);
            request.Reject();
            Action act = () => request.Approve();
            act.Should().Throw<InvalidOperationException>()
                .WithMessage("Only Pending redemptions can be approved.");
        }

        [Fact]
        public void Reject_Should_Throw_When_Status_Is_Not_Pending()
        {
            var request = new RedemptionRequest(Guid.NewGuid(), 50);
            request.Approve();
            Action act = () => request.Reject();
            act.Should().Throw<InvalidOperationException>()
                .WithMessage("Only Pending redemptions can be rejected.");
        }

        [Fact]
        public void MarkCompleted_Should_Throw_When_Status_Is_Not_Approved()
        {
            var request = new RedemptionRequest(Guid.NewGuid(), 50);
            Action act1 = () => request.MarkCompleted(); // Pending -> Completed not allowed
            request.Reject();
            Action act2 = () => request.MarkCompleted(); // Rejected -> Completed not allowed

            act1.Should().Throw<InvalidOperationException>()
                .WithMessage("Only Approved redemptions can be completed.");
            act2.Should().Throw<InvalidOperationException>()
                .WithMessage("Only Approved redemptions can be completed.");
        }

        [Fact]
        public void GetAllowedTransitions_Should_Return_Correct_Statuses()
        {
            var request = new RedemptionRequest(Guid.NewGuid(), 50);

            // Pending
            request.GetAllowedTransitions().Should().BeEquivalentTo(new[] { RedemptionStatus.Approved, RedemptionStatus.Rejected });

            // Approved
            request.Approve();
            request.GetAllowedTransitions().Should().BeEquivalentTo(new[] { RedemptionStatus.Completed });

            // Completed
            request.MarkCompleted();
            request.GetAllowedTransitions().Should().BeEmpty();

            // Rejected
            var rejectedRequest = new RedemptionRequest(Guid.NewGuid(), 50);
            rejectedRequest.Reject();
            rejectedRequest.GetAllowedTransitions().Should().BeEmpty();
        }
    }
}
