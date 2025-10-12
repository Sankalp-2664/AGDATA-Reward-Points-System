using Application.Services;
using Domain.Entities.Product;
using Domain.Entities.Reward;
using Domain.Exceptions;
using Infrastructure.Persistence.Repositories;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Tests.Application.Tests
{
    public class ProductServiceTests
    {
        private ProductService BuildService(
            out InMemoryProductRepository productRepo,
            out InMemoryRewardPointsRepository pointsRepo)
        {
            productRepo = new InMemoryProductRepository();
            pointsRepo = new InMemoryRewardPointsRepository();
            return new ProductService(productRepo, pointsRepo);
        }

        [Fact]
        public async Task AddProduct_ShouldCreateProduct_WhenValid()
        {
            // Arrange
            var service = BuildService(out var productRepo, out var pointsRepo);
            var rp = new RewardPoints(Guid.NewGuid(), 100);
            await pointsRepo.AddAsync(rp);

            // Act
            var product = await service.AddProductAsync("SKU1", "ItemName", rp.Id);

            // Assert
            Assert.NotNull(product);
            Assert.Equal("SKU1", product.SKU);
            Assert.Equal("ItemName", product.Name);

            var fromRepo = await productRepo.GetByIdAsync(product.Id);
            Assert.NotNull(fromRepo);
            Assert.Equal(product.Id, fromRepo.Id);
        }

        [Theory]
        [InlineData(null, "Name")]
        [InlineData("", "Name")]
        public async Task AddProduct_ShouldThrow_WhenSkuIsNullOrEmpty(string sku, string name)
        {
            var service = BuildService(out var productRepo, out var pointsRepo);
            var rp = new RewardPoints(Guid.NewGuid(), 50);
            await pointsRepo.AddAsync(rp);

            await Assert.ThrowsAsync<ArgumentException>(async () =>
                await service.AddProductAsync(sku, name, rp.Id));
        }

        [Theory]
        [InlineData("SKU1", null)]
        [InlineData("SKU1", "")]
        public async Task AddProduct_ShouldThrow_WhenNameIsNullOrEmpty(string sku, string name)
        {
            var service = BuildService(out var productRepo, out var pointsRepo);
            var rp = new RewardPoints(Guid.NewGuid(), 50);
            await pointsRepo.AddAsync(rp);

            await Assert.ThrowsAsync<ArgumentException>(async () =>
                await service.AddProductAsync(sku, name, rp.Id));
        }

        [Fact]
        public async Task AddProduct_ShouldThrow_WhenRewardPointsDoesNotExist()
        {
            var service = BuildService(out var productRepo, out var pointsRepo);
            var missingRpId = Guid.NewGuid();

            await Assert.ThrowsAsync<ArgumentException>(async () =>
                await service.AddProductAsync("SKU2", "ItemName", missingRpId));
        }

        [Fact]
        public async Task GetById_ShouldReturnProduct_WhenExists()
        {
            var service = BuildService(out var productRepo, out var pointsRepo);
            var rp = new RewardPoints(Guid.NewGuid(), 100);
            await pointsRepo.AddAsync(rp);

            var product = await service.AddProductAsync("SKUX", "ProdX", rp.Id);

            var fetched = await productRepo.GetByIdAsync(product.Id);
            Assert.NotNull(fetched);
            Assert.Equal("SKUX", fetched.SKU);
        }

        [Fact]
        public async Task GetById_ShouldReturnNull_WhenNotExists()
        {
            var service = BuildService(out var productRepo, out var pointsRepo);
            var notFound = await productRepo.GetByIdAsync(Guid.NewGuid());
            Assert.Null(notFound);
        }

        [Fact]
        public async Task GetBySku_ShouldReturnProduct_WhenExists()
        {
            var service = BuildService(out var productRepo, out var pointsRepo);
            var rp = new RewardPoints(Guid.NewGuid(), 100);
            await pointsRepo.AddAsync(rp);

            var product = await service.AddProductAsync("SKUTEST", "ProdTest", rp.Id);

            var fetched = await productRepo.GetBySkuAsync("SKUTEST");
            Assert.NotNull(fetched);
            Assert.Equal(product.Id, fetched.Id);
        }

        [Fact]
        public async Task GetBySku_ShouldReturnNull_WhenNotExists()
        {
            var service = BuildService(out var productRepo, out var pointsRepo);
            var fetched = await productRepo.GetBySkuAsync("NOEXIST");
            Assert.Null(fetched);
        }

        [Fact]
        public async Task ListProducts_ShouldReturnAllAddedProducts()
        {
            var service = BuildService(out var productRepo, out var pointsRepo);
            var rp = new RewardPoints(Guid.NewGuid(), 100);
            await pointsRepo.AddAsync(rp);

            await service.AddProductAsync("SKU1", "Name1", rp.Id);
            await service.AddProductAsync("SKU2", "Name2", rp.Id);

            var list = await productRepo.ListAsync();
            Assert.NotNull(list);
            Assert.Equal(2, list.Count());
        }

        [Fact]
        public async Task AddProduct_ShouldTrimSkuAndName()
        {
            var service = BuildService(out var productRepo, out var pointsRepo);
            var rp = new RewardPoints(Guid.NewGuid(), 100);
            await pointsRepo.AddAsync(rp);

            var product = await service.AddProductAsync("  SKU3  ", "  Product Name  ", rp.Id);

            Assert.Equal("SKU3", product.SKU);
            Assert.Equal("Product Name", product.Name);
        }

        [Fact]
        public async Task AddProduct_ShouldThrow_WhenSkuIsDuplicate()
        {
            var service = BuildService(out var productRepo, out var pointsRepo);
            var rp = new RewardPoints(Guid.NewGuid(), 100);
            await pointsRepo.AddAsync(rp);

            await service.AddProductAsync("DUPSKU", "Product1", rp.Id);

            await Assert.ThrowsAsync<ArgumentException>(() =>
                service.AddProductAsync("DUPSKU", "Product2", rp.Id));
        }

        [Fact]
        public async Task AddProduct_ShouldThrow_WhenSkuIsDuplicateWithDifferentCase()
        {
            var service = BuildService(out var productRepo, out var pointsRepo);
            var rp = new RewardPoints(Guid.NewGuid(), 100);
            await pointsRepo.AddAsync(rp);

            await service.AddProductAsync("casesku", "Product1", rp.Id);

            await Assert.ThrowsAsync<ArgumentException>(() =>
                service.AddProductAsync("CaseSKU", "Product2", rp.Id));
        }

        [Fact]
        public async Task AddProduct_ShouldAllowMultipleProductsWithDifferentSkus()
        {
            var service = BuildService(out var productRepo, out var pointsRepo);
            var rp = new RewardPoints(Guid.NewGuid(), 100);
            await pointsRepo.AddAsync(rp);

            var p1 = await service.AddProductAsync("SKU-A", "ProductA", rp.Id);
            var p2 = await service.AddProductAsync("SKU-B", "ProductB", rp.Id);

            Assert.NotEqual(p1.Id, p2.Id);
        }

        [Fact]
        public async Task AddProduct_ShouldThrow_IfRewardPointsIsZero()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                var rp = new RewardPoints(Guid.NewGuid(), 0);
            });
        }

        [Fact]
        public async Task AddProduct_ShouldThrow_IfRewardPointsIsNegative()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                var rp = new RewardPoints(Guid.NewGuid(), -50);
            });
        }


        [Fact]
        public async Task GetBySku_ShouldReturnNull_IfSkuIsNullOrEmpty()
        {
            var service = BuildService(out var productRepo, out var pointsRepo);

            var result1 = await productRepo.GetBySkuAsync(null!);
            var result2 = await productRepo.GetBySkuAsync("");

            Assert.Null(result1);
            Assert.Null(result2);
        }

        [Fact]
        public async Task GetBySku_ShouldBeCaseInsensitive()
        {
            var service = BuildService(out var productRepo, out var pointsRepo);
            var rp = new RewardPoints(Guid.NewGuid(), 10);
            await pointsRepo.AddAsync(rp);

            var product = await service.AddProductAsync("TestSKU", "Prod", rp.Id);

            var fetched = await productRepo.GetBySkuAsync("testsku");
            Assert.NotNull(fetched);
            Assert.Equal(product.Id, fetched.Id);
        }

        [Fact]
        public async Task ListProducts_ShouldReturnEmptyList_IfNoneAdded()
        {
            var service = BuildService(out var productRepo, out var pointsRepo);

            var list = await productRepo.ListAsync();

            Assert.NotNull(list);
            Assert.Empty(list);
        }

        [Fact]
        public async Task AddProduct_ShouldThrow_IfSkuContainsWhitespaceOnly()
        {
            var service = BuildService(out var productRepo, out var pointsRepo);
            var rp = new RewardPoints(Guid.NewGuid(), 10);
            await pointsRepo.AddAsync(rp);

            await Assert.ThrowsAsync<ArgumentException>(() =>
                service.AddProductAsync("    ", "ValidName", rp.Id));
        }

        [Fact]
        public async Task AddProduct_ShouldThrow_IfNameContainsWhitespaceOnly()
        {
            var service = BuildService(out var productRepo, out var pointsRepo);
            var rp = new RewardPoints(Guid.NewGuid(), 10);
            await pointsRepo.AddAsync(rp);

            await Assert.ThrowsAsync<ArgumentException>(() =>
                service.AddProductAsync("ValidSKU", "    ", rp.Id));
        }

        [Fact]
        public async Task AddProduct_ShouldAllowVeryLongSkuAndName()
        {
            var service = BuildService(out var productRepo, out var pointsRepo);
            var rp = new RewardPoints(Guid.NewGuid(), 10);
            await pointsRepo.AddAsync(rp);

            var longSku = new string('A', 256);
            var longName = new string('B', 512);

            var product = await service.AddProductAsync(longSku, longName, rp.Id);

            Assert.Equal(longSku, product.SKU);
            Assert.Equal(longName, product.Name);
        }

        [Fact]
        public async Task AddProduct_ShouldThrow_IfRewardPointsRepositoryThrows()
        {
            // Simulate repo throwing on AddAsync
            var productRepo = new InMemoryProductRepository();
            var pointsRepo = new ThrowingRewardPointsRepository();
            var service = new ProductService(productRepo, pointsRepo);

            var rpId = Guid.NewGuid();

            await Assert.ThrowsAsync<Exception>(async () =>
                await service.AddProductAsync("SKU1", "Name1", rpId));
        }

        // Helper repo that throws on GetByIdAsync
        class ThrowingRewardPointsRepository : InMemoryRewardPointsRepository
        {
            public override Task<RewardPoints?> GetByIdAsync(Guid id)
            {
                throw new Exception("Simulated repository failure");
            }
        }

        

    }
}
