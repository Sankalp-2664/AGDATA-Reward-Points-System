using Application.Services;
using Domain.Entities;
using Infrastructure.Persistence.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests.Application.Tests
{
    public class RedemptionServiceTests
    {
        [Fact]
        public async Task Redemption_ShouldDeductPointsAndUpdateStock()
        {
            // Arrange
            var redemptionRepo = new InMemoryRedemptionRecordRepository();
            var processRepo = new InMemoryRedemptionProcessRepository();
            var accountRepo = new InMemoryUserAccountRepository();
            var productRepo = new InMemoryProductRepository();
            var inventoryRepo = new InMemoryProductInventoryRepository();
            var pointsRepo = new InMemoryRewardPointsRepository();
            var transactionRepo = new InMemoryRewardTransactionRepository();

            var redemptionService = new RedemptionService(
                redemptionRepo, processRepo,
                accountRepo, productRepo, inventoryRepo,
                pointsRepo, transactionRepo
            );

            // Create user + account with points
            var account = new UserAccount(Guid.NewGuid());
            await accountRepo.AddAsync(account);

            var rewardPoints = new RewardPoints(Guid.NewGuid(), 50);
            await pointsRepo.AddAsync(rewardPoints);

            var product = new ProductInfo(Guid.NewGuid(), "SKU1", "Coffee Mug", rewardPoints.Id);
            await productRepo.AddAsync(product);

            var inventory = new ProductInventory(product.Id, 5);
            await inventoryRepo.AddAsync(inventory);

            // give the user some points
            var tx = new RewardTransaction(Guid.NewGuid(), account.UserId, 50, "Init points");
            account.AddPoints(50, tx);
            await accountRepo.UpdateAsync(account);

            // Act
            var redemption = await redemptionService.RequestRedemptionAsync(account.UserId, product.Id);
            await redemptionService.ApproveRedemptionAsync(redemption.Id);
            await redemptionService.CompleteRedemptionAsync(redemption.Id);

            // Assert
            var updated = await accountRepo.GetByUserIdAsync(account.UserId);
            Assert.Equal(0, updated!.RewardBalance);

            var inv = await inventoryRepo.GetByProductIdAsync(product.Id);
            Assert.Equal(4, inv!.StockQuantity);
        }
    }
}
