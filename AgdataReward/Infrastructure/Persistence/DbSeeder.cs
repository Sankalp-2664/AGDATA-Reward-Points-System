using Application.Interfaces;
using Domain.Entities.User;
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

        Console.WriteLine("DbSeeder: starting...");

        // ===============================
        // 1. Seed Roles
        // ===============================
        if (!await context.Set<Role>().AnyAsync())
        {
            var adminRole = new Role("Admin");
            var userRole = new Role("User");

            await context.AddRangeAsync(adminRole, userRole);
            await context.SaveChangesAsync();

            Console.WriteLine("Roles seeded.");
        }

        // ===============================
        // 2. Seed Admin User
        // ===============================
        if (!await context.UserProfiles.AnyAsync())
        {
            var adminRole = await context.Set<Role>()
                .FirstAsync(r => r.Name == "Admin");

            var email = new Email("admin@agdata.com");
            var employeeId = new EmployeeId("EMP-0001");

            var adminUser = new UserProfile(
                employeeId,
                email,
                "System",
                "Administrator"
            );

            // Create account
            var account = new UserAccount(adminUser.Id);

            var passwordResult = passwordHasher.Hash("Admin@123");
            account.SetCredentials(passwordResult.Hash, passwordResult.Salt);

            adminUser.AttachAccount(account);

            // Assign Admin role
            adminUser.AssignRole(adminRole);

            await context.UserProfiles.AddAsync(adminUser);
            await context.SaveChangesAsync();

            Console.WriteLine("Admin user seeded successfully.");
        }
        else
        {
            Console.WriteLine("Users already exist. Skipping user seeding.");
        }
    }
}
