using Application.Services;
using Domain.Entities;
using Domain.Enums;
using Domain.Exceptions;
using Infrastructure.Persistence.Repositories;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Tests.Application.Tests
{
    public class RedemptionServiceNegativeTests
    {
        private static RedemptionService BuildService(
            out InMemoryUserAccountRepository accountRepo,
            out InMemoryProductRepository productRepo,
            out InMemoryProductInventoryRepository inventoryRepo,
            out InMemoryRewardPointsRepository pointsRepo,
            out InMemoryRedemptionRecordRepository recordRepo,
            out InMemoryRedemptionProcessRepository processRepo,
            out InMemoryRewardTransactionRepository transactionRepo)
        {
            recordRepo = new InMemoryRedemptionRecordRepository();
            processRepo = new InMemoryRedemptionProcessRepository();
            accountRepo = new InMemoryUserAccountRepository();
            productRepo = new InMemoryProductRepository();
            inventoryRepo = new InMemoryProductInventoryRepository();
            pointsRepo = new InMemoryRewardPointsRepository();
            transactionRepo = new InMemoryRewardTransactionRepository();

            return new RedemptionService(
                recordRepo, processRepo,
                accountRepo, productRepo, inventoryRepo,
                pointsRepo, transactionRepo
            );
        }

        [Fact]
        public async Task RequestRedemption_ShouldFail_IfUserHasInsufficientPoints()
        {
            // Arrange
            var service = BuildService(
                out var accountRepo, out var productRepo, out var inventoryRepo,
                out var pointsRepo, out var recordRepo, out var processRepo, out var transactionRepo);

            var account = new UserAccount(Guid.NewGuid());
            await accountRepo.AddAsync(account);

            var rewardPoints = new RewardPoints(Guid.NewGuid(), 100); // product requires 100 points
            await pointsRepo.AddAsync(rewardPoints);

            var product = new ProductInfo(Guid.NewGuid(), "SKU1", "T-Shirt", rewardPoints.Id);
            await productRepo.AddAsync(product);

            var inventory = new ProductInventory(product.Id, 5);
            await inventoryRepo.AddAsync(inventory);

            // Act + Assert
            await Assert.ThrowsAsync<InsufficientPointsException>(
                () => service.RequestRedemptionAsync(account.UserId, product.Id));
        }

        [Fact]
        public async Task RequestRedemption_ShouldFail_IfProductOutOfStock()
        {
            // Arrange
            var service = BuildService(
                out var accountRepo, out var productRepo, out var inventoryRepo,
                out var pointsRepo, out var recordRepo, out var processRepo, out var transactionRepo);

            var account = new UserAccount(Guid.NewGuid());
            var tx = new RewardTransaction(Guid.NewGuid(), account.UserId, 200, "Init");
            account.AddPoints(200, tx);
            await accountRepo.AddAsync(account);

            var rewardPoints = new RewardPoints(Guid.NewGuid(), 100);
            await pointsRepo.AddAsync(rewardPoints);

            var product = new ProductInfo(Guid.NewGuid(), "SKU2", "Coffee Mug", rewardPoints.Id);
            await productRepo.AddAsync(product);

            var inventory = new ProductInventory(product.Id, 0); // no stock
            await inventoryRepo.AddAsync(inventory);

            // Act + Assert
            await Assert.ThrowsAsync<InvalidRedemptionException>(
                () => service.RequestRedemptionAsync(account.UserId, product.Id));
        }

        [Fact]
        public async Task RejectRedemption_ShouldUpdateStatus()
        {
            // Arrange
            var service = BuildService(
                out var accountRepo, out var productRepo, out var inventoryRepo,
                out var pointsRepo, out var recordRepo, out var processRepo, out var transactionRepo);

            var account = new UserAccount(Guid.NewGuid());
            var tx = new RewardTransaction(Guid.NewGuid(), account.UserId, 200, "Init");
            account.AddPoints(200, tx);
            await accountRepo.AddAsync(account);

            var rewardPoints = new RewardPoints(Guid.NewGuid(), 100);
            await pointsRepo.AddAsync(rewardPoints);

            var product = new ProductInfo(Guid.NewGuid(), "SKU3", "Headphones", rewardPoints.Id);
            await productRepo.AddAsync(product);

            var inventory = new ProductInventory(product.Id, 5);
            await inventoryRepo.AddAsync(inventory);

            var record = await service.RequestRedemptionAsync(account.UserId, product.Id);

            // Act
            await service.RejectRedemptionAsync(record.Id);

            // Assert
            var process = await processRepo.GetByIdAsync(record.Id);
            Assert.Equal(RedemptionStatus.Rejected, process!.Status);
        }

        [Fact]
        public async Task ApproveRedemption_ShouldUpdateStatus()
        {
            // Arrange
            var service = BuildService(
                out var accountRepo, out var productRepo, out var inventoryRepo,
                out var pointsRepo, out var recordRepo, out var processRepo, out var transactionRepo);

            var account = new UserAccount(Guid.NewGuid());
            var tx = new RewardTransaction(Guid.NewGuid(), account.UserId, 200, "Init");
            account.AddPoints(200, tx);
            await accountRepo.AddAsync(account);

            var rewardPoints = new RewardPoints(Guid.NewGuid(), 100);
            await pointsRepo.AddAsync(rewardPoints);

            var product = new ProductInfo(Guid.NewGuid(), "SKU4", "Backpack", rewardPoints.Id);
            await productRepo.AddAsync(product);

            var inventory = new ProductInventory(product.Id, 5);
            await inventoryRepo.AddAsync(inventory);

            var record = await service.RequestRedemptionAsync(account.UserId, product.Id);

            // Act
            await service.ApproveRedemptionAsync(record.Id);

            // Assert
            var process = await processRepo.GetByIdAsync(record.Id);
            Assert.Equal(RedemptionStatus.Approved, process!.Status);
        }

        [Fact]
        public async Task CompleteRedemption_ShouldFail_IfNotApproved()
        {
            // Arrange
            var service = BuildService(
                out var accountRepo, out var productRepo, out var inventoryRepo,
                out var pointsRepo, out var recordRepo, out var processRepo, out var transactionRepo);

            var account = new UserAccount(Guid.NewGuid());
            var tx = new RewardTransaction(Guid.NewGuid(), account.UserId, 200, "Init");
            account.AddPoints(200, tx);
            await accountRepo.AddAsync(account);

            var rewardPoints = new RewardPoints(Guid.NewGuid(), 100);
            await pointsRepo.AddAsync(rewardPoints);

            var product = new ProductInfo(Guid.NewGuid(), "SKU5", "Laptop Sleeve", rewardPoints.Id);
            await productRepo.AddAsync(product);

            var inventory = new ProductInventory(product.Id, 5);
            await inventoryRepo.AddAsync(inventory);

            var record = await service.RequestRedemptionAsync(account.UserId, product.Id);

            // Act + Assert
            await Assert.ThrowsAsync<InvalidOperationException>(
                () => service.CompleteRedemptionAsync(record.Id));

        }
    }
}
