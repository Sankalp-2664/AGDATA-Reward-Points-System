using Application.Interfaces;
using Application.Services;
using Domain.Entities.Event;
using Domain.Entities.Product;
using Domain.Entities.Reward;
using Domain.Enums;
using Domain.ValueObjects;
using Infrastructure.Persistence.Repositories;
using Infrastructure.Security;   // 👈 for PasswordHasher

namespace Api.Server.Milestone1;

public static class Milestone1Program
{
    public static async Task Run()
    {
        Console.WriteLine("--- Milestone 1 Demo Starting ---");

        // Initialize in-memory repositories
        var userRepo = new InMemoryUserRepository();
        var accountRepo = new InMemoryUserAccountRepository();
        var eventDefRepo = new InMemoryEventDefinitionRepository();
        var eventRuleRepo = new InMemoryEventRewardRuleRepository();
        var eventInstanceRepo = new InMemoryEventInstanceRepository();
        var productRepo = new InMemoryProductRepository();
        var inventoryRepo = new InMemoryProductInventoryRepository();
        var rewardPointsRepo = new InMemoryRewardPointsRepository();
        var transactionRepo = new InMemoryRewardTransactionRepository();
        var redemptionRepo = new InMemoryRedemptionRecordRepository();
        var redemptionRequestRepo = new InMemoryRedemptionRequestRepository();

        // Password hasher (same as used in real app)
        IPasswordHasher passwordHasher = new PasswordHasher();

        // Initialize services with updated constructor
        IUserService userService = new UserService(userRepo, accountRepo, passwordHasher);
        IEventService eventService = new EventService(
            eventDefRepo,
            eventRuleRepo,
            eventInstanceRepo,
            accountRepo,
            transactionRepo,
            rewardPointsRepo);
        IRedemptionService redemptionService = new RedemptionService(
            redemptionRepo,
            redemptionRequestRepo,
            accountRepo,
            productRepo,
            inventoryRepo,
            rewardPointsRepo,
            transactionRepo);

        // 1. Register a user (now with password)
        var user = await userService.RegisterUserAsync(
            "E123",
            "user@mail.com",
            "Sankalp",
            "Chakre",
            UserRole.User,
            "User@123" // demo password
        );

        Console.WriteLine($"Registered user: {user.FirstName} ({user.Email.Value})");

        // 2. Create an event
        var eventDef = await eventService.CreateEventAsync("HACK", "Hackathon");
        Console.WriteLine($"Created event: {eventDef.Title}");

        // 3. Add reward points and rule
        var rewardPoints = new RewardPoints(Guid.NewGuid(), 100);
        await rewardPointsRepo.AddAsync(rewardPoints);
        await eventService.AddRewardRuleAsync(eventDef.Id, rank: 1, rewardPointsId: rewardPoints.Id);

        // 4. Add an instance of the event
        var instance = new EventInstance(Guid.NewGuid(), eventDef.Id);
        await eventInstanceRepo.AddAsync(instance);

        // 5. Assign the user as a winner
        await eventService.AssignWinnerAsync(instance.Id, user.Id, rank: 1);
        Console.WriteLine($"{user.FirstName} assigned as winner for event instance.");

        // 6. Add a product and inventory
        var product = new ProductInformation(Guid.NewGuid(), new SKU("SKU1"), "Coffee Mug", rewardPoints.Id);
        await productRepo.AddAsync(product);
        var inventory = new ProductInventory(Guid.NewGuid(), product.Id, 5);
        await inventoryRepo.AddAsync(inventory);

        // 7. Request redemption
        var redemption = await redemptionService.RequestRedemptionAsync(user.Id, product.Id);
        Console.WriteLine($"Redemption requested for product: {product.Name}");

        // 8. Approve and complete redemption
        await redemptionService.ApproveRedemptionAsync(redemption.Id);
        await redemptionService.CompleteRedemptionAsync(redemption.Id);
        Console.WriteLine($"Redemption completed for product: {product.Name}");

        // 9. Show updated balance and stock
        var updatedAccount = await userService.GetUserAccountAsync(user.Id);
        var updatedInventory = await inventoryRepo.GetByProductIdAsync(product.Id);

        Console.WriteLine($"User {user.FirstName} now has balance: {updatedAccount!.RewardBalance}");
        Console.WriteLine($"Product '{product.Name}' remaining stock: {updatedInventory!.StockQuantity}");
        Console.WriteLine("--- Milestone 1 Demo Completed ---");
    }
}
