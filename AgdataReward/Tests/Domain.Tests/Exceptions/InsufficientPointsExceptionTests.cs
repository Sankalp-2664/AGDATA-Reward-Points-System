using Domain.Exceptions;
using FluentAssertions;
using Xunit;
using System;

namespace Tests.Domain.Tests.Exceptions
{
    public class InsufficientPointsExceptionTests
    {
        [Fact]
        public void Constructor_Should_SetPropertiesCorrectly()
        {
            // Arrange
            int available = 100;
            int required = 150;

            // Act
            var ex = new InsufficientPointsException(available, required);

            // Assert
            ex.CurrentBalance.Should().Be(available);
            ex.Attempted.Should().Be(required);
            ex.Message.Should().Be("Insufficient points. Available: 100, Required: 150.");
        }

        [Fact]
        public void Should_Inherit_From_DomainException()
        {
            // Arrange
            var ex = new InsufficientPointsException(50, 75);

            // Assert
            ex.Should().BeAssignableTo<DomainException>();
        }

        [Fact]
        public void Should_BeAssignable_To_Exception()
        {
            // Arrange
            var ex = new InsufficientPointsException(10, 20);

            // Assert
            ex.Should().BeAssignableTo<Exception>();
        }

        [Fact]
        public void Should_Handle_ZeroValues_Correctly()
        {
            // Arrange
            int available = 0;
            int required = 0;

            // Act
            var ex = new InsufficientPointsException(available, required);

            // Assert
            ex.CurrentBalance.Should().Be(0);
            ex.Attempted.Should().Be(0);
            ex.Message.Should().Be("Insufficient points. Available: 0, Required: 0.");
        }
    }
}
