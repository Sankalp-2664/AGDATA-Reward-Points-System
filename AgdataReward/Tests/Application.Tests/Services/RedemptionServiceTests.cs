using Application.Services;
using Domain.Entities.Product;
using Domain.Entities.Redemption;
using Domain.Entities.Reward;
using Domain.Entities.User;
using Domain.Enums;
using Domain.Exceptions;
using Domain.ValueObjects;
using Infrastructure.Persistence.Repositories;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Tests.Application.Tests.Services
{
    public class RedemptionServiceTests
    {
        private static RedemptionService BuildService(
            out InMemoryUserAccountRepository accountRepo,
            out InMemoryProductRepository productRepo,
            out InMemoryProductInventoryRepository inventoryRepo,
            out InMemoryRewardPointsRepository pointsRepo,
            out InMemoryRedemptionRecordRepository recordRepo,
            out InMemoryRedemptionRequestRepository requestRepo,
            out InMemoryRewardTransactionRepository transactionRepo)
        {
            recordRepo = new InMemoryRedemptionRecordRepository();
            requestRepo = new InMemoryRedemptionRequestRepository();
            accountRepo = new InMemoryUserAccountRepository();
            productRepo = new InMemoryProductRepository();
            inventoryRepo = new InMemoryProductInventoryRepository();
            pointsRepo = new InMemoryRewardPointsRepository();
            transactionRepo = new InMemoryRewardTransactionRepository();

            return new RedemptionService(
                recordRepo, requestRepo,
                accountRepo, productRepo, inventoryRepo,
                pointsRepo, transactionRepo
            );
        }

        [Fact]
        public async Task Redemption_ShouldDeductPointsAndUpdateStock()
        {
            // Arrange
            var service = BuildService(
                out var accountRepo, out var productRepo, out var inventoryRepo,
                out var pointsRepo, out var recordRepo, out var requestRepo, out var transactionRepo);

            var account = new UserAccount(Guid.NewGuid());
            await accountRepo.AddAsync(account);

            var rewardPoints = new RewardPoints(Guid.NewGuid(), 50);
            await pointsRepo.AddAsync(rewardPoints);

            var product = new ProductInformation(Guid.NewGuid(), new SKU("SKU1"), "Coffee Mug", rewardPoints.Id);
            await productRepo.AddAsync(product);

            var inventory = new ProductInventory(Guid.NewGuid(), product.Id, 5);
            await inventoryRepo.AddAsync(inventory);

            var tx = new RewardTransaction(account.UserId, 100, "Initial points", TransactionType.Credit);
            account.AddPoints(100, tx);
            await accountRepo.UpdateAsync(account);

            // Act
            var redemption = await service.RequestRedemptionAsync(account.UserId, product.Id);
            await service.ApproveRedemptionAsync(redemption.Id);
            await service.CompleteRedemptionAsync(redemption.Id);

            // Assert
            var updatedAccount = await accountRepo.GetByUserIdAsync(account.UserId);
            Assert.Equal(50, updatedAccount!.RewardBalance);

            var updatedInventory = await inventoryRepo.GetByProductIdAsync(product.Id);
            Assert.Equal(4, updatedInventory!.StockQuantity);

            var process = await requestRepo.GetByIdAsync(redemption.Id);
            Assert.Equal(RedemptionStatus.Completed, process!.Status);
        }

        [Fact]
        public async Task RequestRedemption_ShouldThrow_IfInsufficientPoints()
        {
            var service = BuildService(
                out var accountRepo, out var productRepo, out var inventoryRepo,
                out var pointsRepo, out var recordRepo, out var requestRepo, out var transactionRepo);

            var account = new UserAccount(Guid.NewGuid());
            await accountRepo.AddAsync(account);

            var rewardPoints = new RewardPoints(Guid.NewGuid(), 200);
            await pointsRepo.AddAsync(rewardPoints);

            var product = new ProductInformation(Guid.NewGuid(), new SKU("SKU2"), "T-Shirt", rewardPoints.Id);
            await productRepo.AddAsync(product);

            var inventory = new ProductInventory(Guid.NewGuid(), product.Id, 3);
            await inventoryRepo.AddAsync(inventory);

            // user only has 50 points
            var tx = new RewardTransaction(account.UserId, 50, "Init", TransactionType.Credit);
            account.AddPoints(50, tx);
            await accountRepo.UpdateAsync(account);

            await Assert.ThrowsAsync<InsufficientPointsException>(() =>
                service.RequestRedemptionAsync(account.UserId, product.Id));
        }

        [Fact]
        public async Task RequestRedemption_ShouldThrow_IfProductOutOfStock()
        {
            var service = BuildService(
                out var accountRepo, out var productRepo, out var inventoryRepo,
                out var pointsRepo, out var recordRepo, out var requestRepo, out var transactionRepo);

            var account = new UserAccount(Guid.NewGuid());
            var tx = new RewardTransaction(account.UserId, 500, "Init", TransactionType.Credit);
            account.AddPoints(500, tx);
            await accountRepo.AddAsync(account);

            var rewardPoints = new RewardPoints(Guid.NewGuid(), 100);
            await pointsRepo.AddAsync(rewardPoints);

            var product = new ProductInformation(Guid.NewGuid(), new SKU("SKU3"), "Water Bottle", rewardPoints.Id);
            await productRepo.AddAsync(product);

            var inventory = new ProductInventory(Guid.NewGuid(), product.Id, 0);
            await inventoryRepo.AddAsync(inventory);

            await Assert.ThrowsAsync<InvalidRedemptionException>(() =>
                service.RequestRedemptionAsync(account.UserId, product.Id));
        }

        [Fact]
        public async Task RequestRedemption_ShouldThrow_IfDuplicatePendingExists()
        {
            var service = BuildService(
                out var accountRepo, out var productRepo, out var inventoryRepo,
                out var pointsRepo, out var recordRepo, out var requestRepo, out var transactionRepo);

            var account = new UserAccount(Guid.NewGuid());
            var tx = new RewardTransaction(account.UserId, 500, "Init", TransactionType.Credit);
            account.AddPoints(500, tx);
            await accountRepo.AddAsync(account);

            var rewardPoints = new RewardPoints(Guid.NewGuid(), 100);
            await pointsRepo.AddAsync(rewardPoints);

            var product = new ProductInformation(Guid.NewGuid(), new SKU("SKU4"), "Mouse Pad", rewardPoints.Id);
            await productRepo.AddAsync(product);

            var inventory = new ProductInventory(Guid.NewGuid(), product.Id, 5);
            await inventoryRepo.AddAsync(inventory);

            var redemption = await service.RequestRedemptionAsync(account.UserId, product.Id);

            // Second attempt should fail
            await Assert.ThrowsAsync<InvalidRedemptionException>(() =>
                service.RequestRedemptionAsync(account.UserId, product.Id));
        }

        [Fact]
        public async Task CompleteRedemption_ShouldThrow_IfNotApproved()
        {
            var service = BuildService(
                out var accountRepo, out var productRepo, out var inventoryRepo,
                out var pointsRepo, out var recordRepo, out var requestRepo, out var transactionRepo);

            var account = new UserAccount(Guid.NewGuid());
            var tx = new RewardTransaction(account.UserId, 100, "Init", TransactionType.Credit);
            account.AddPoints(100, tx);
            await accountRepo.AddAsync(account);

            var rewardPoints = new RewardPoints(Guid.NewGuid(), 50);
            await pointsRepo.AddAsync(rewardPoints);

            var product = new ProductInformation(Guid.NewGuid(), new SKU("SKU5"), "Notebook", rewardPoints.Id);
            await productRepo.AddAsync(product);

            var inventory = new ProductInventory(Guid.NewGuid(), product.Id, 10);
            await inventoryRepo.AddAsync(inventory);

            var redemption = await service.RequestRedemptionAsync(account.UserId, product.Id);

            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                service.CompleteRedemptionAsync(redemption.Id));
        }

        [Fact]
        public async Task RejectRedemption_ShouldUpdateStatusToRejected()
        {
            var service = BuildService(
                out var accountRepo, out var productRepo, out var inventoryRepo,
                out var pointsRepo, out var recordRepo, out var requestRepo, out var transactionRepo);

            var account = new UserAccount(Guid.NewGuid());
            var tx = new RewardTransaction(account.UserId, 100, "Init", TransactionType.Credit);
            account.AddPoints(100, tx);
            await accountRepo.AddAsync(account);

            var rewardPoints = new RewardPoints(Guid.NewGuid(), 50);
            await pointsRepo.AddAsync(rewardPoints);

            var product = new ProductInformation(Guid.NewGuid(), new SKU("SKU6"), "Sticker", rewardPoints.Id);
            await productRepo.AddAsync(product);

            var inventory = new ProductInventory(Guid.NewGuid(), product.Id, 3);
            await inventoryRepo.AddAsync(inventory);

            var redemption = await service.RequestRedemptionAsync(account.UserId, product.Id);

            await service.RejectRedemptionAsync(redemption.Id);

            var process = await requestRepo.GetByIdAsync(redemption.Id);
            Assert.Equal(RedemptionStatus.Rejected, process!.Status);
        }
    }
}
