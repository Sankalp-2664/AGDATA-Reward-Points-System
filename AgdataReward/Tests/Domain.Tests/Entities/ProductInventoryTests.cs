using Domain.Entities.Product;
using FluentAssertions;

namespace Tests.Domain.Tests.Entities;

public class ProductInventoryTests
{
    [Fact]
    public void Constructor_Should_Throw_When_ProductId_Is_Empty()
    {
        // Arrange
        var id = Guid.NewGuid();

        // Act
        Action act = () => new ProductInventory(id, Guid.Empty, 10);

        // Assert
        act.Should().Throw<ArgumentException>().WithMessage("*ProductId is required*");
    }

    [Fact]
    public void Constructor_Should_Throw_When_Stock_Is_Negative()
    {
        // Arrange
        var id = Guid.NewGuid();

        // Act
        Action act = () => new ProductInventory(Guid.NewGuid(), Guid.NewGuid(), -5);

        // Assert
        act.Should().Throw<ArgumentException>().WithMessage("*Stock cannot be negative*");
    }

    [Fact]
    public void Constructor_Should_Initialize_Values_Correctly()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var stock = 100;

        // Act
        var inventory = new ProductInventory(Guid.NewGuid(), productId, stock);

        // Assert
        inventory.ProductId.Should().Be(productId);
        inventory.StockQuantity.Should().Be(stock);
        inventory.IsActive.Should().BeTrue();
    }

    [Fact]
    public void IncreaseStock_Should_Increase_Stock_By_Qty()
    {
        // Arrange
        var inventory = new ProductInventory(Guid.NewGuid(), Guid.NewGuid(), 10);

        // Act
        inventory.IncreaseStock(5);

        // Assert
        inventory.StockQuantity.Should().Be(15);
    }

    [Fact]
    public void IncreaseStock_Should_Throw_When_Qty_Is_NonPositive()
    {
        // Arrange
        var inventory = new ProductInventory(Guid.NewGuid(), Guid.NewGuid(), 10);

        // Act
        Action act = () => inventory.IncreaseStock(0);

        // Assert
        act.Should().Throw<ArgumentException>().WithMessage("*qty must be positive*");
    }

    [Fact]
    public void ReduceStock_Should_Decrease_Stock_By_Qty()
    {
        // Arrange
        var inventory = new ProductInventory(Guid.NewGuid(), Guid.NewGuid(), 20);

        // Act
        inventory.ReduceStock(5);

        // Assert
        inventory.StockQuantity.Should().Be(15);
    }

    [Fact]
    public void ReduceStock_Should_Throw_When_Qty_Is_NonPositive()
    {
        // Arrange
        var inventory = new ProductInventory(Guid.NewGuid(), Guid.NewGuid(), 20);

        // Act
        Action act = () => inventory.ReduceStock(0);

        // Assert
        act.Should().Throw<ArgumentException>().WithMessage("*qty must be positive*");
    }

    [Fact]
    public void ReduceStock_Should_Throw_When_Qty_Exceeds_Stock()
    {
        // Arrange
        var inventory = new ProductInventory(Guid.NewGuid(), Guid.NewGuid(), 5);

        // Act
        Action act = () => inventory.ReduceStock(10);

        // Assert
        act.Should().Throw<InvalidOperationException>().WithMessage("*Insufficient stock*");
    }

    [Fact]
    public void Deactivate_Should_Set_IsActive_To_False()
    {
        // Arrange
        var inventory = new ProductInventory(Guid.NewGuid(), Guid.NewGuid(), 10);

        // Act
        inventory.Deactivate();

        // Assert
        inventory.IsActive.Should().BeFalse();
    }
}