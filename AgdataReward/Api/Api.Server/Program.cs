using Application.Interfaces;
using Application.Services;
using Domain.Entities;
using Infrastructure.Persistence.Repositories;
using System;
using System.Threading.Tasks;

class Program
{
    static async Task Main()
    {
        Console.WriteLine("=== AGDATA Reward Points System - Test Harness ===\n");

        // -----------------------------
        // Setup In-Memory Repositories
        // -----------------------------
        IUserRepository userRepo = new InMemoryUserRepository();
        IUserAccountRepository accountRepo = new InMemoryUserAccountRepository();
        IProductRepository productRepo = new InMemoryProductRepository();
        IProductInventoryRepository inventoryRepo = new InMemoryProductInventoryRepository();
        IRewardPointsRepository rewardPointsRepo = new InMemoryRewardPointsRepository();
        IEventDefinitionRepository eventDefRepo = new InMemoryEventDefinitionRepository();
        IEventRewardRuleRepository eventRuleRepo = new InMemoryEventRewardRuleRepository();
        IEventInstanceRepository eventInstanceRepo = new InMemoryEventInstanceRepository();
        IRewardTransactionRepository transactionRepo = new InMemoryRewardTransactionRepository();
        IRedemptionRecordRepository redemptionRecordRepo = new InMemoryRedemptionRecordRepository();
        IRedemptionProcessRepository redemptionProcessRepo = new InMemoryRedemptionProcessRepository();

        // -----------------------------
        // Setup Services
        // -----------------------------
        IUserService userService = new UserService(userRepo, accountRepo);
        IProductService productService = new ProductService(productRepo, rewardPointsRepo);
        IInventoryService inventoryService = new InventoryService(inventoryRepo);
        IEventService eventService = new EventService(
            eventDefRepo, eventRuleRepo, eventInstanceRepo,
            accountRepo, transactionRepo, rewardPointsRepo
        );
        IRedemptionService redemptionService = new RedemptionService(
            redemptionRecordRepo, redemptionProcessRepo,
            accountRepo, productRepo, inventoryRepo,
            rewardPointsRepo, transactionRepo
        );
        ITransactionService transactionService = new TransactionService(transactionRepo);

        // -----------------------------
        // 1. Register a user
        // -----------------------------
        var user = await userService.RegisterUserAsync("EMP001", "sankalp@agdata.com", "sankalp", "chakre");
        Console.WriteLine($"User registered: {user.FirstName} {user.LastName} ({user.Email})");

        // -----------------------------
        // 2. Admin creates RewardPoints configs
        // -----------------------------
        var rp100 = new RewardPoints(Guid.NewGuid(), 100);
        var rp75 = new RewardPoints(Guid.NewGuid(), 75);
        var rp50 = new RewardPoints(Guid.NewGuid(), 50);
        await rewardPointsRepo.AddAsync(rp100);
        await rewardPointsRepo.AddAsync(rp75);
        await rewardPointsRepo.AddAsync(rp50);

        // -----------------------------
        // 3. Admin creates product + inventory
        // -----------------------------
        var product = await productService.AddProductAsync("SKU001", "Pen", rp100.Id);
        await inventoryService.UpdateStockAsync(product.Id, 5);
        Console.WriteLine($"Product added: {product.Name} (requires {rp100.PointsValue} points, stock 5)");

        // -----------------------------
        // 4. Admin creates event + rules
        // -----------------------------
        var hackathon = await eventService.CreateEventAsync("HACK2025", "Hackathon 2025");
        await eventService.AddRewardRuleAsync(hackathon.Id, 1, rp100.Id);
        await eventService.AddRewardRuleAsync(hackathon.Id, 2, rp75.Id);
        await eventService.AddRewardRuleAsync(hackathon.Id, 3, rp50.Id);

        // Create event instance (occurrence)
        var instance = new EventInstance(Guid.NewGuid(), hackathon.Id);
        await eventInstanceRepo.AddAsync(instance);

        Console.WriteLine($"Event created: {hackathon.Title} ({hackathon.Code})");

        // -----------------------------
        // 5. Assign winner 
        // -----------------------------
        await eventService.AssignWinnerAsync(instance.Id, user.Id, 1);
        Console.WriteLine("Sankalp awarded 100 points for 1st place.");

        // Check balance
        var account = await accountRepo.GetByUserIdAsync(user.Id);
        Console.WriteLine($"Balance after event: {account?.RewardBalance} points");

        // -----------------------------
        // 6. Redeem product
        // -----------------------------
        var redemption = await redemptionService.RequestRedemptionAsync(user.Id, product.Id);
        await redemptionService.ApproveRedemptionAsync(redemption.Id);
        await redemptionService.CompleteRedemptionAsync(redemption.Id);

        Console.WriteLine($"sankalp redeemed product: {product.Name}");

        // Check balance again
        account = await accountRepo.GetByUserIdAsync(user.Id);
        Console.WriteLine($"Balance after redemption: {account?.RewardBalance} points");

        // -----------------------------
        // 7. Transaction History
        // -----------------------------
        var transactions = await transactionService.GetUserTransactionsAsync(user.Id);
        Console.WriteLine("\n--- Transaction History ---");
        foreach (var t in transactions)
        {
            Console.WriteLine($"{t.TransactionId} | {t.Notes} | {t.PointsDelta} points");
        }
    }
}
