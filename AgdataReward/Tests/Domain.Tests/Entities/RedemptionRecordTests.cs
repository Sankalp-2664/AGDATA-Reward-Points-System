using Domain.Entities.Redemption;
using FluentAssertions;
using System;
using Xunit;

namespace Tests.Domain.Tests.Entities
{
    public class RedemptionRecordTests
    {
        [Fact]
        public void Constructor_Should_Throw_When_Id_Is_Empty()
        {
            var userId = Guid.NewGuid();
            var productId = Guid.NewGuid();

            Action act = () => new RedemptionRecord(Guid.Empty, userId, productId);

            act.Should().Throw<ArgumentException>()
                .WithMessage("Id cannot be empty.");
        }

        [Fact]
        public void Constructor_Should_Throw_When_UserId_Is_Empty()
        {
            var id = Guid.NewGuid();
            var productId = Guid.NewGuid();

            Action act = () => new RedemptionRecord(id, Guid.Empty, productId);

            act.Should().Throw<ArgumentException>()
                .WithMessage("UserId cannot be empty.");
        }

        [Fact]
        public void Constructor_Should_Throw_When_ProductId_Is_Empty()
        {
            var id = Guid.NewGuid();
            var userId = Guid.NewGuid();

            Action act = () => new RedemptionRecord(id, userId, Guid.Empty);

            act.Should().Throw<ArgumentException>()
                .WithMessage("ProductId cannot be empty.");
        }

        [Fact]
        public void Constructor_Should_Initialize_Values_Correctly()
        {
            var id = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var productId = Guid.NewGuid();

            var record = new RedemptionRecord(id, userId, productId);

            record.Id.Should().Be(id);
            record.UserId.Should().Be(userId);
            record.ProductId.Should().Be(productId);
            record.RedeemedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
            record.User.Should().BeNull();
            record.Product.Should().BeNull();
        }
    }
}
