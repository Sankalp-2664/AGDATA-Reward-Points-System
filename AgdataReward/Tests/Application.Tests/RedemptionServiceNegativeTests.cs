using Application.Services;
using Domain.Entities.Product;
using Domain.Entities.Reward;
using Domain.Entities.User;
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

			var product = new ProductInfo(Guid.NewGuid(), "SKU1", "T-Shirt", rewardPoints.Id);
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

			var product = new ProductInfo(Guid.NewGuid(), "SKU2", "Coffee Mug", rewardPoints.Id);
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

			var product = new ProductInfo(Guid.NewGuid(), "SKU3", "Headphones", rewardPoints.Id);
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

			var product = new ProductInfo(Guid.NewGuid(), "SKU4", "Backpack", rewardPoints.Id);
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

			var product = new ProductInfo(Guid.NewGuid(), "SKU5", "Laptop Sleeve", rewardPoints.Id);
			await productRepo.AddAsync(product);

			var inventory = new ProductInventory(Guid.NewGuid(), product.Id, 5);
			await inventoryRepo.AddAsync(inventory);

			var redemptionRecord = await service.RequestRedemptionAsync(userId, product.Id);

			// Act & Assert
			await Assert.ThrowsAsync<InvalidOperationException>(() =>
				service.CompleteRedemptionAsync(redemptionRecord.Id));
		}
	}
}
