using Domain.Exceptions;
using FluentAssertions;
using Xunit;
using System;

namespace Tests.Domain.Tests.Exceptions
{
    public class DomainExceptionTests
    {
        [Fact]
        public void Constructor_Should_SetMessageCorrectly()
        {
            // Arrange
            var message = "Something went wrong.";

            // Act
            var exception = new DomainException(message);

            // Assert
            exception.Message.Should().Be(message);
            exception.Should().BeOfType<DomainException>();
        }

        [Fact]
        public void Constructor_Should_Handle_NullMessage_Gracefully()
        {
            var exception = new DomainException(null!);

            exception.Message.Should().NotBeNull(); // base sets default string
            exception.Should().BeOfType<DomainException>();
        }

        [Fact]
        public void DomainException_Should_BeAssignableTo_Exception()
        {
            // Arrange
            var exception = new DomainException("Error");

            // Assert
            exception.Should().BeAssignableTo<Exception>();
        }
    }
}
