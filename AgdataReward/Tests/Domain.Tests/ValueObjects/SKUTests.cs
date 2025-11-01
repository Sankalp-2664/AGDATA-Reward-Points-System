using System;
using Domain.ValueObjects;
using FluentAssertions;
using Xunit;

namespace Tests.Domain.Tests.ValueObjects
{
    public class SKUTests
    {
        [Fact]
        public void Constructor_Should_Create_Valid_SKU()
        {
            // Arrange
            var value = "prod-123";

            // Act
            var sku = new SKU(value);

            // Assert
            sku.Value.Should().Be("PROD-123"); // Should normalize to uppercase
            sku.ToString().Should().Be("PROD-123");
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData(null)]
        public void Constructor_Should_Throw_When_EmptyOrWhitespace(string? invalid)
        {
            // Act
            Action act = () => new SKU(invalid!);

            // Assert
            act.Should().Throw<ArgumentException>()
                .WithMessage("SKU cannot be empty or whitespace.*");
        }

        [Fact]
        public void Constructor_Should_Throw_When_Less_Than_4_Characters()
        {
            // Act
            Action act = () => new SKU("A12");

            // Assert
            act.Should().Throw<ArgumentException>()
                .WithMessage("SKU must be at least 4 characters long.*");
        }

        [Theory]
        [InlineData("SKU#123")]
        [InlineData("SKU 123")]
        [InlineData("sku@id")]
        public void Constructor_Should_Throw_When_Invalid_Format(string invalid)
        {
            // Act
            Action act = () => new SKU(invalid);

            // Assert
            act.Should().Throw<ArgumentException>()
                .WithMessage("SKU must only contain letters, numbers, or hyphens.*");
        }

        [Fact]
        public void Equals_Should_Return_True_For_Same_Value_Ignoring_Case()
        {
            // Arrange
            var sku1 = new SKU("ABC-123");
            var sku2 = new SKU("abc-123");

            // Act & Assert
            sku1.Equals(sku2).Should().BeTrue();
            (sku1 == sku2).Should().BeTrue();
            (sku1 != sku2).Should().BeFalse();
        }

        [Fact]
        public void Equals_Should_Return_False_For_Different_Values()
        {
            // Arrange
            var sku1 = new SKU("ABC-123");
            var sku2 = new SKU("XYZ-789");

            // Act & Assert
            sku1.Equals(sku2).Should().BeFalse();
            (sku1 == sku2).Should().BeFalse();
            (sku1 != sku2).Should().BeTrue();
        }

        [Fact]
        public void GetHashCode_Should_Be_CaseInsensitive()
        {
            // Arrange
            var sku1 = new SKU("abc-123");
            var sku2 = new SKU("ABC-123");

            // Act
            var hash1 = sku1.GetHashCode();
            var hash2 = sku2.GetHashCode();

            // Assert
            hash1.Should().Be(hash2);
        }

        [Fact]
        public void Implicit_Conversion_ToString_Should_Return_Value()
        {
            // Arrange
            var sku = new SKU("Prod-001");

            // Act
            string value = sku;

            // Assert
            value.Should().Be("PROD-001");
        }

        [Fact]
        public void Explicit_Conversion_FromString_Should_Create_SKU()
        {
            // Arrange
            string value = "abc-777";

            // Act
            var sku = (SKU)value;

            // Assert
            sku.Value.Should().Be("ABC-777");
        }

        [Fact]
        public void Equals_Should_Return_False_When_Compared_To_Null()
        {
            // Arrange
            var sku = new SKU("TEST-123");

            // Act
            var result = sku.Equals(null);

            // Assert
            result.Should().BeFalse();
        }
    }
}
