using System;
using Domain.ValueObjects;
using FluentAssertions;
using Xunit;

namespace Tests.Domain.Tests.ValueObjects
{
    public class EmployeeIdTests
    {
        [Fact]
        public void Constructor_Should_Create_EmployeeId_When_Valid()
        {
            // Arrange
            var value = "EMP123";

            // Act
            var empId = new EmployeeId(value);

            // Assert
            empId.Value.Should().Be(value);
            empId.ToString().Should().Be(value);
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData(null)]
        public void Constructor_Should_Throw_When_Empty(string invalid)
        {
            // Act
            Action act = () => new EmployeeId(invalid);

            // Assert
            act.Should().Throw<ArgumentException>()
                .WithMessage("Employee ID cannot be empty.*");
        }

        [Theory]
        [InlineData("A")]
        [InlineData("12")]
        public void Constructor_Should_Throw_When_Less_Than_Three_Characters(string invalid)
        {
            // Act
            Action act = () => new EmployeeId(invalid);

            // Assert
            act.Should().Throw<ArgumentException>()
                .WithMessage("Employee ID must be at least 3 characters long.*");
        }

        [Fact]
        public void Equals_Should_Return_True_For_Same_Value_Ignoring_Case()
        {
            // Arrange
            var id1 = new EmployeeId("EMP001");
            var id2 = new EmployeeId("emp001");

            // Act
            var result = id1.Equals(id2);

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void Equals_Should_Return_False_For_Different_Values()
        {
            // Arrange
            var id1 = new EmployeeId("EMP001");
            var id2 = new EmployeeId("EMP002");

            // Act
            var result = id1.Equals(id2);

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void GetHashCode_Should_Be_CaseInsensitive()
        {
            // Arrange
            var id1 = new EmployeeId("EMP123");
            var id2 = new EmployeeId("emp123");

            // Act
            var hash1 = id1.GetHashCode();
            var hash2 = id2.GetHashCode();

            // Assert
            hash1.Should().Be(hash2);
        }
    }
}
