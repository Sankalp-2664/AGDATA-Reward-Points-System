using System;
using Domain.ValueObjects;
using FluentAssertions;
using Xunit;

namespace Tests.Domain.Tests.ValueObjects
{
    public class EmailTests
    {
        [Fact]
        public void Constructor_Should_Create_Email_When_Valid()
        {
            // Arrange
            var value = "john.doe@agdata.com";

            // Act
            var email = new Email(value);

            // Assert
            email.Value.Should().Be(value);
            email.ToString().Should().Be(value);
        }

        [Fact]
        public void Constructor_Should_Trim_Whitespace()
        {
            // Arrange
            var value = "   jane.doe@agdata.com   ";

            // Act
            var email = new Email(value);

            // Assert
            email.Value.Should().Be("jane.doe@agdata.com");
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData(null)]
        public void Constructor_Should_Throw_When_Empty(string invalid)
        {
            // Act
            Action act = () => new Email(invalid);

            // Assert
            act.Should().Throw<ArgumentException>()
                .WithMessage("Email cannot be empty.*");
        }

        [Fact]
        public void Constructor_Should_Throw_When_Missing_At_Symbol()
        {
            // Arrange
            var invalid = "john.doeagdata.com";

            // Act
            Action act = () => new Email(invalid);

            // Assert
            act.Should().Throw<ArgumentException>()
                .WithMessage("Invalid email format.*");
        }

        [Theory]
        [InlineData("john.doe@gmail.com")]
        [InlineData("john.doe@agdata.co")]
        [InlineData("john.doe@notagdata.com")]
        public void Constructor_Should_Throw_When_Not_Agdata_Domain(string invalid)
        {
            // Act
            Action act = () => new Email(invalid);

            // Assert
            act.Should().Throw<ArgumentException>()
                .WithMessage("Only AGDATA employees can register.*");
        }

        [Fact]
        public void Equals_Should_Return_True_For_Same_Email_Ignoring_Case()
        {
            // Arrange
            var email1 = new Email("John.Doe@agdata.com");
            var email2 = new Email("john.doe@AGDATA.com");

            // Act & Assert
            email1.Equals(email2).Should().BeTrue();
            (email1 == email2).Should().BeFalse(); // no operator overload
        }

        [Fact]
        public void Equals_Should_Return_False_For_Different_Emails()
        {
            // Arrange
            var email1 = new Email("john.doe@agdata.com");
            var email2 = new Email("jane.doe@agdata.com");

            // Act
            var result = email1.Equals(email2);

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void GetHashCode_Should_Be_CaseInsensitive()
        {
            // Arrange
            var email1 = new Email("JOHN.DOE@AGDATA.COM");
            var email2 = new Email("john.doe@agdata.com");

            // Act
            var hash1 = email1.GetHashCode();
            var hash2 = email2.GetHashCode();

            // Assert
            hash1.Should().Be(hash2);
        }
    }
}
