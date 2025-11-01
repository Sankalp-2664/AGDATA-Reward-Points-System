using Domain.Entities.Event;
using FluentAssertions;
using System;
using Xunit;

namespace Tests.Domain.Tests.Entities
{
    public class EventInstanceTests
    {
        [Fact]
        public void Constructor_Should_Initialize_Correctly()
        {
            // Arrange
            var eventId = Guid.NewGuid();
            var id = Guid.NewGuid();

            // Act
            var instance = new EventInstance(id, eventId);

            // Assert
            instance.Id.Should().Be(id);
            instance.EventId.Should().Be(eventId);
            instance.WinnerUserId.Should().BeNull();
            instance.Rank.Should().BeNull();
        }

        [Fact]
        public void Constructor_Should_Throw_When_Id_Is_Empty()
        {
            // Arrange
            var eventId = Guid.NewGuid();

            // Act
            Action act = () => new EventInstance(Guid.Empty, eventId);

            // Assert
            act.Should().Throw<ArgumentException>()
               .WithMessage("*Id cannot be empty*");
        }

        [Fact]
        public void Constructor_Should_Throw_When_EventId_Is_Empty()
        {
            // Arrange
            var id = Guid.NewGuid();

            // Act
            Action act = () => new EventInstance(id, Guid.Empty);

            // Assert
            act.Should().Throw<ArgumentException>()
               .WithMessage("*EventId cannot be empty*");
        }

        [Fact]
        public void AssignWinner_Should_Set_WinnerId_And_Rank()
        {
            // Arrange
            var instance = new EventInstance(Guid.NewGuid(), Guid.NewGuid());
            var userId = Guid.NewGuid();
            var rank = 1;

            // Act
            instance.AssignWinner(userId, rank);

            // Assert
            instance.WinnerUserId.Should().Be(userId);
            instance.Rank.Should().Be(rank);
        }

        [Fact]
        public void AssignWinner_Should_Throw_When_UserId_Is_Empty()
        {
            // Arrange
            var instance = new EventInstance(Guid.NewGuid(), Guid.NewGuid());

            // Act
            Action act = () => instance.AssignWinner(Guid.Empty, 1);

            // Assert
            act.Should().Throw<ArgumentException>()
               .WithMessage("*Invalid user ID*");
        }

        [Fact]
        public void AssignWinner_Should_Throw_When_Rank_Is_NonPositive()
        {
            // Arrange
            var instance = new EventInstance(Guid.NewGuid(), Guid.NewGuid());
            var userId = Guid.NewGuid();

            // Act
            Action act = () => instance.AssignWinner(userId, 0);

            // Assert
            act.Should().Throw<ArgumentOutOfRangeException>()
               .WithMessage("*Rank must be greater than zero*");
        }
    }
}
