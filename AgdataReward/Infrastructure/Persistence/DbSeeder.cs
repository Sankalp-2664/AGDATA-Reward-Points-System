using Application.Interfaces;
using Domain.Entities.User;
using Domain.Enums;
using Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Persistence;

public static class DbSeeder
{
    public static async Task SeedAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<RewardDbContext>();
        var passwordHasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher>();

        Console.WriteLine(" DbSeeder: starting...");

        if (await context.UserProfiles.AnyAsync())
        {
            Console.WriteLine("DbSeeder: users already exist, skipping seeding.");
            return;
        }

        Console.WriteLine(" DbSeeder: seeding admin user...");

        var email = new Email("admin@agdata.com");
        var employeeId = new EmployeeId("EMP-000");

        var user = new UserProfile(
            employeeId,
            email,
            "System",
            "Administrator",
            UserRole.Admin
        );

        var userAccount = new UserAccount(user.Id);

        var result = passwordHasher.Hash("Admin@123");
        var hash = result.Hash;
        var salt = result.Salt;

        userAccount.SetCredentials(hash, salt);
        user.AttachAccount(userAccount);

        await context.UserProfiles.AddAsync(user);
        await context.SaveChangesAsync();

        Console.WriteLine(" DbSeeder: admin user created successfully.");
    }
}
