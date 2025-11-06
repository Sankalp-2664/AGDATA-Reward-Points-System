using Domain.Entities.Product;
using Domain.ValueObjects;
using FluentAssertions;

namespace Tests.Domain.Tests.Entities;

public class ProductInformationTests
{
    [Fact]
    public void Constructor_Should_Throw_When_SKU_Is_Null()
    {
        // Arrange
        var id = Guid.NewGuid();
        var name = "Product Name";
        var rewardPointsId = Guid.NewGuid();

        // Act
        Action act = () => new ProductInformation(id, null!, name, rewardPointsId);

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .WithMessage("*SKU is required*");
    }

    [Fact]
    public void Constructor_Should_Throw_When_Name_Is_NullOrWhitespace()
    {
        // Arrange
        var id = Guid.NewGuid();
        var sku = new SKU("ABC123");
        var rewardPointsId = Guid.NewGuid();

        // Act
        Action act1 = () => new ProductInformation(id, sku, null!, rewardPointsId);
        Action act2 = () => new ProductInformation(id, sku, "", rewardPointsId);
        Action act3 = () => new ProductInformation(id, sku, "   ", rewardPointsId);

        // Assert
        act1.Should().Throw<ArgumentException>().WithMessage("*Product name is required*");
        act2.Should().Throw<ArgumentException>().WithMessage("*Product name is required*");
        act3.Should().Throw<ArgumentException>().WithMessage("*Product name is required*");
    }

    [Fact]
    public void Constructor_Should_Throw_When_RewardPointsId_Is_Empty()
    {
        // Arrange
        var id = Guid.NewGuid();
        var sku = new SKU("ABC123");
        var name = "Product Name";

        // Act
        Action act = () => new ProductInformation(id, sku, name, Guid.Empty);

        // Assert
        act.Should().Throw<ArgumentException>().WithMessage("*RewardPointsId cannot be empty*");
    }

    [Fact]
    public void Constructor_Should_Initialize_Values_Correctly()
    {
        // Arrange
        var id = Guid.NewGuid();
        var sku = new SKU("ABC123");
        var name = "Sample Product";
        var rewardPointsId = Guid.NewGuid();

        // Act
        var product = new ProductInformation(id, sku, name, rewardPointsId);

        // Assert
        product.Id.Should().Be(id);
        product.SKU.Should().Be(sku);
        product.Name.Should().Be(name);
        product.RewardPointsId.Should().Be(rewardPointsId);
    }
}