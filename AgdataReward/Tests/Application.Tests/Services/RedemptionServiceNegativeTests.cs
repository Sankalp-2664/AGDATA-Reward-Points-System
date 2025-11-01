using Application.Services;
using Domain.Entities.Product;
using Domain.Entities.Redemption;
using Domain.Entities.Reward;
using Domain.Entities.User;
using Domain.Enums;
using Domain.ValueObjects;
using Domain.Exceptions;
using Infrastructure.Persistence.Repositories;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Tests.Application.Tests.Services
{
	public class RedemptionServiceNegativeTests
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
		public async Task RequestRedemption_ShouldFail_IfUserHasInsufficientPoints()
		{
			// Arrange
			var service = BuildService(
				out var accountRepo, out var productRepo, out var inventoryRepo,
				out var pointsRepo, out var recordRepo, out var requestRepo, out var transactionRepo);

			var userId = Guid.NewGuid();
			var account = new UserAccount(userId);
			await accountRepo.AddAsync(account);

			var rewardPoints = new RewardPoints(Guid.NewGuid(), 100); // Product costs 100 points
			await pointsRepo.AddAsync(rewardPoints);

			var product = new ProductInformation(Guid.NewGuid(), new SKU("SKU1"), "T-Shirt", rewardPoints.Id);
			await productRepo.AddAsync(product);

			var inventory = new ProductInventory(Guid.NewGuid(), product.Id, 5);
			await inventoryRepo.AddAsync(inventory);

			// Act & Assert
			await Assert.ThrowsAsync<InsufficientPointsException>(() =>
				service.RequestRedemptionAsync(userId, product.Id));
		}

		[Fact]
		public async Task RequestRedemption_ShouldFail_IfProductOutOfStock()
		{
			// Arrange
			var service = BuildService(
				out var accountRepo, out var productRepo, out var inventoryRepo,
				out var pointsRepo, out var recordRepo, out var requestRepo, out var transactionRepo);

			var userId = Guid.NewGuid();
			var account = new UserAccount(userId);
			var initTx = new RewardTransaction(userId, 200, "Initial points", TransactionType.Credit);
			account.AddPoints(200, initTx);
			await accountRepo.AddAsync(account);

			var rewardPoints = new RewardPoints(Guid.NewGuid(), 100);
			await pointsRepo.AddAsync(rewardPoints);

			var product = new ProductInformation(Guid.NewGuid(), new SKU("SKU2"), "Coffee Mug", rewardPoints.Id);
			await productRepo.AddAsync(product);

			var inventory = new ProductInventory(Guid.NewGuid(), product.Id, 0); // Out of stock
			await inventoryRepo.AddAsync(inventory);

			// Act & Assert
			await Assert.ThrowsAsync<InvalidRedemptionException>(() =>
				service.RequestRedemptionAsync(userId, product.Id));
		}

		[Fact]
		public async Task RejectRedemption_ShouldUpdateStatus()
		{
			// Arrange
			var service = BuildService(
				out var accountRepo, out var productRepo, out var inventoryRepo,
				out var pointsRepo, out var recordRepo, out var requestRepo, out var transactionRepo);

			var userId = Guid.NewGuid();
			var account = new UserAccount(userId);
			var initTx = new RewardTransaction(userId, 200, "Initial points", TransactionType.Credit);
			account.AddPoints(200, initTx);
			await accountRepo.AddAsync(account);

			var rewardPoints = new RewardPoints(Guid.NewGuid(), 100);
			await pointsRepo.AddAsync(rewardPoints);

			var product = new ProductInformation(Guid.NewGuid(), new SKU("SKU3"), "Headphones", rewardPoints.Id);
			await productRepo.AddAsync(product);

			var inventory = new ProductInventory(Guid.NewGuid(), product.Id, 5);
			await inventoryRepo.AddAsync(inventory);

			var redemptionRecord = await service.RequestRedemptionAsync(userId, product.Id);

			// Act
			await service.RejectRedemptionAsync(redemptionRecord.Id);

			// Assert
			var process = await requestRepo.GetByIdAsync(redemptionRecord.Id);
			Assert.NotNull(process);
			Assert.Equal(RedemptionStatus.Rejected, process.Status);
		}

		[Fact]
		public async Task ApproveRedemption_ShouldUpdateStatus()
		{
			// Arrange
			var service = BuildService(
				out var accountRepo, out var productRepo, out var inventoryRepo,
				out var pointsRepo, out var recordRepo, out var requestRepo, out var transactionRepo);

			var userId = Guid.NewGuid();
			var account = new UserAccount(userId);
			var initTx = new RewardTransaction(userId, 200, "Initial points", TransactionType.Credit);
			account.AddPoints(200, initTx);
			await accountRepo.AddAsync(account);

			var rewardPoints = new RewardPoints(Guid.NewGuid(), 100);
			await pointsRepo.AddAsync(rewardPoints);

			var product = new ProductInformation(Guid.NewGuid(), new SKU("SKU4"), "Backpack", rewardPoints.Id);
			await productRepo.AddAsync(product);

			var inventory = new ProductInventory(Guid.NewGuid(), product.Id, 5);
			await inventoryRepo.AddAsync(inventory);

			var redemptionRecord = await service.RequestRedemptionAsync(userId, product.Id);

			// Act
			await service.ApproveRedemptionAsync(redemptionRecord.Id);

			// Assert
			var process = await requestRepo.GetByIdAsync(redemptionRecord.Id);
			Assert.NotNull(process);
			Assert.Equal(RedemptionStatus.Approved, process.Status);
		}

		[Fact]
		public async Task CompleteRedemption_ShouldFail_IfNotApproved()
		{
			// Arrange
			var service = BuildService(
				out var accountRepo, out var productRepo, out var inventoryRepo,
				out var pointsRepo, out var recordRepo, out var requestRepo, out var transactionRepo);

			var userId = Guid.NewGuid();
			var account = new UserAccount(userId);
			var initTx = new RewardTransaction(userId, 200, "Initial points", TransactionType.Credit);
			account.AddPoints(200, initTx);
			await accountRepo.AddAsync(account);

			var rewardPoints = new RewardPoints(Guid.NewGuid(), 100);
			await pointsRepo.AddAsync(rewardPoints);

			var product = new ProductInformation(Guid.NewGuid(), new SKU("SKU5"), "Laptop Sleeve", rewardPoints.Id);
			await productRepo.AddAsync(product);

			var inventory = new ProductInventory(Guid.NewGuid(), product.Id, 5);
			await inventoryRepo.AddAsync(inventory);

			var redemptionRecord = await service.RequestRedemptionAsync(userId, product.Id);

			// Act & Assert
			await Assert.ThrowsAsync<InvalidOperationException>(() =>
				service.CompleteRedemptionAsync(redemptionRecord.Id));
		}

        [Fact]
        public async Task GetPendingOrActiveByUserAndProductAsync_ShouldReturnOnlyPendingOrApproved()
        {
            // Arrange
            var recordRepo = new InMemoryRedemptionRecordRepository();
            var requestRepo = new InMemoryRedemptionRequestRepository();

            var userId = Guid.NewGuid();
            var productId = Guid.NewGuid();

            // Create redemption records
            var recordPending = new RedemptionRecord(Guid.NewGuid(), userId, productId);
            var recordApproved = new RedemptionRecord(Guid.NewGuid(), userId, productId);
            var recordRejected = new RedemptionRecord(Guid.NewGuid(), userId, productId);

            await recordRepo.AddAsync(recordPending);
            await recordRepo.AddAsync(recordApproved);
            await recordRepo.AddAsync(recordRejected);

            // Create requests linked to records
            var pendingRequest = new RedemptionRequest(recordPending.Id, 100); // Pending by default
            var approvedRequest = new RedemptionRequest(recordApproved.Id, 50);
            approvedRequest.Approve();
            var rejectedRequest = new RedemptionRequest(recordRejected.Id, 20);
            rejectedRequest.Reject();

            await requestRepo.UpdateAsync(pendingRequest);
            await requestRepo.UpdateAsync(approvedRequest);
            await requestRepo.UpdateAsync(rejectedRequest);

            // Act
            var allRecords = await recordRepo.GetAllAsync();
            var results = (await requestRepo.GetPendingOrActiveByUserAndProductAsync(userId, productId, allRecords)).ToList();

            // Assert
            Assert.Contains(results, r => r.Id == pendingRequest.Id);
            Assert.Contains(results, r => r.Id == approvedRequest.Id);
            Assert.DoesNotContain(results, r => r.Id == rejectedRequest.Id);
        }
    }
}
