using Domain.Entities.Product;
using Domain.Entities.Reward;
using Domain.ValueObjects;
using FluentAssertions;
using System;
using Xunit;

namespace Tests.Domain.Tests.Entities
{
    public class ProductInformationTests
    {
        [Fact]
        public void Constructor_Should_Throw_When_SKU_Is_Null()
        {
            Action act = () => new ProductInformation(Guid.NewGuid(), null!, "Product Name", Guid.NewGuid());
            act.Should().Throw<ArgumentNullException>()
                .WithMessage("*SKU is required*");
        }

        [Fact]
        public void Constructor_Should_Throw_When_Name_Is_NullOrWhitespace()
        {
            var sku = new SKU("ABC123");
            Action act1 = () => new ProductInformation(Guid.NewGuid(), sku, null!, Guid.NewGuid());
            Action act2 = () => new ProductInformation(Guid.NewGuid(), sku, "", Guid.NewGuid());
            Action act3 = () => new ProductInformation(Guid.NewGuid(), sku, "   ", Guid.NewGuid());

            act1.Should().Throw<ArgumentException>().WithMessage("*Product name is required*");
            act2.Should().Throw<ArgumentException>().WithMessage("*Product name is required*");
            act3.Should().Throw<ArgumentException>().WithMessage("*Product name is required*");
        }

        [Fact]
        public void Constructor_Should_Throw_When_RewardPointsId_Is_Empty()
        {
            var sku = new SKU("ABC123");
            Action act = () => new ProductInformation(Guid.NewGuid(), sku, "Product Name", Guid.Empty);
            act.Should().Throw<ArgumentException>().WithMessage("*RewardPointsId cannot be empty*");
        }

        [Fact]
        public void Constructor_Should_Initialize_Values_Correctly()
        {
            var id = Guid.NewGuid();
            var sku = new SKU("ABC123");
            var name = "Sample Product";
            var rewardPointsId = Guid.NewGuid();

            var product = new ProductInformation(id, sku, name, rewardPointsId);

            product.Id.Should().Be(id);
            product.SKU.Should().Be(sku);
            product.Name.Should().Be(name);
            product.RewardPointsId.Should().Be(rewardPointsId);
        }
    }
}
