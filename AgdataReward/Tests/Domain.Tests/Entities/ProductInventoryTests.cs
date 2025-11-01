using Domain.Entities.Product;
using FluentAssertions;
using System;
using Xunit;

namespace Tests.Domain.Tests.Entities
{
    public class ProductInventoryTests
    {
        [Fact]
        public void Constructor_Should_Throw_When_ProductId_Is_Empty()
        {
            Action act = () => new ProductInventory(Guid.NewGuid(), Guid.Empty, 10);
            act.Should().Throw<ArgumentException>().WithMessage("*ProductId is required*");
        }

        [Fact]
        public void Constructor_Should_Throw_When_Stock_Is_Negative()
        {
            Action act = () => new ProductInventory(Guid.NewGuid(), Guid.NewGuid(), -5);
            act.Should().Throw<ArgumentException>().WithMessage("*Stock cannot be negative*");
        }

        [Fact]
        public void Constructor_Should_Initialize_Values_Correctly()
        {
            var productId = Guid.NewGuid();
            var stock = 100;
            var inventory = new ProductInventory(Guid.NewGuid(), productId, stock);

            inventory.ProductId.Should().Be(productId);
            inventory.StockQuantity.Should().Be(stock);
            inventory.IsActive.Should().BeTrue();
        }

        [Fact]
        public void IncreaseStock_Should_Increase_Stock_By_Qty()
        {
            var inventory = new ProductInventory(Guid.NewGuid(), Guid.NewGuid(), 10);
            inventory.IncreaseStock(5);
            inventory.StockQuantity.Should().Be(15);
        }

        [Fact]
        public void IncreaseStock_Should_Throw_When_Qty_Is_NonPositive()
        {
            var inventory = new ProductInventory(Guid.NewGuid(), Guid.NewGuid(), 10);
            Action act = () => inventory.IncreaseStock(0);
            act.Should().Throw<ArgumentException>().WithMessage("*qty must be positive*");
        }

        [Fact]
        public void ReduceStock_Should_Decrease_Stock_By_Qty()
        {
            var inventory = new ProductInventory(Guid.NewGuid(), Guid.NewGuid(), 20);
            inventory.ReduceStock(5);
            inventory.StockQuantity.Should().Be(15);
        }

        [Fact]
        public void ReduceStock_Should_Throw_When_Qty_Is_NonPositive()
        {
            var inventory = new ProductInventory(Guid.NewGuid(), Guid.NewGuid(), 20);
            Action act = () => inventory.ReduceStock(0);
            act.Should().Throw<ArgumentException>().WithMessage("*qty must be positive*");
        }

        [Fact]
        public void ReduceStock_Should_Throw_When_Qty_Exceeds_Stock()
        {
            var inventory = new ProductInventory(Guid.NewGuid(), Guid.NewGuid(), 5);
            Action act = () => inventory.ReduceStock(10);
            act.Should().Throw<InvalidOperationException>().WithMessage("*Insufficient stock*");
        }

        [Fact]
        public void Deactivate_Should_Set_IsActive_To_False()
        {
            var inventory = new ProductInventory(Guid.NewGuid(), Guid.NewGuid(), 10);
            inventory.Deactivate();
            inventory.IsActive.Should().BeFalse();
        }
    }
}
