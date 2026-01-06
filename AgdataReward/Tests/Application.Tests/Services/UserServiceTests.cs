using Application.Services;
using Domain.Entities.User;
using Domain.Exceptions;
using Infrastructure.Persistence.Repositories;
using Infrastructure.Security;

namespace Tests.Application.Tests.Services;

public class UserServiceTests
{
    private static UserService CreateService(
        InMemoryUserRepository? userRepo = null,
        InMemoryUserAccountRepository? accountRepo = null)
    {
        userRepo ??= new InMemoryUserRepository();
        accountRepo ??= new InMemoryUserAccountRepository();

        var roleRepo = new InMemoryRoleRepository();
        roleRepo.AddAsync(new Role("User")).Wait();

        var passwordHasher = new PasswordHasher();

        return new UserService(
            userRepo,
            accountRepo,
            roleRepo,
            passwordHasher);
    }

    [Fact]
    public async Task RegisterUser_ShouldCreateUserAndAccount()
    {
        var service = CreateService();

        var user = await service.RegisterUserAsync(
            "EMP001",
            "sankalp@agdata.com",
            "sankalp",
            "chakre",
            "User",
            "User@123"
        );

        Assert.NotNull(user);
        Assert.Equal("sankalp@agdata.com", user.Email.Value);

        var account = await service.GetUserAccountAsync(user.Id);
        Assert.NotNull(account);
        Assert.Equal(0, account!.RewardBalance);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public async Task RegisterUser_ShouldThrow_IfEmployeeIdIsNullOrEmpty(string employeeId)
    {
        var service = CreateService();

        Func<Task> act = () => service.RegisterUserAsync(
            employeeId!,
            "user@agdata.com",
            "Sankalp",
            "C",
            "User",
            "User@123"
        );

        await Assert.ThrowsAsync<ArgumentException>(act);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("invalidemail")]
    public async Task RegisterUser_ShouldThrow_IfEmailIsInvalid(string email)
    {
        var service = CreateService();

        Func<Task> act = () => service.RegisterUserAsync(
            "EMP123",
            email!,
            "Sankalp",
            "C",
            "User",
            "User@123"
        );

        await Assert.ThrowsAsync<ArgumentException>(act);
    }

    [Fact]
    public async Task RegisterUser_ShouldThrow_IfEmployeeIdAlreadyExists()
    {
        var service = CreateService();

        await service.RegisterUserAsync(
            "EMP001",
            "first@agdata.com",
            "First",
            "User",
            "User",
            "User@123"
        );

        Func<Task> act = () => service.RegisterUserAsync(
            "EMP001",
            "second@agdata.com",
            "Second",
            "User",
            "User",
            "User@123"
        );

        await Assert.ThrowsAsync<DuplicateUserException>(act);
    }

    [Fact]
    public async Task RegisterUser_ShouldThrow_IfEmailAlreadyExists()
    {
        var service = CreateService();

        await service.RegisterUserAsync(
            "EMP001",
            "duplicate@agdata.com",
            "Alice",
            "Smith",
            "User",
            "User@123"
        );

        Func<Task> act = () => service.RegisterUserAsync(
            "EMP002",
            "duplicate@agdata.com",
            "Bob",
            "Brown",
            "User",
            "User@123"
        );

        await Assert.ThrowsAsync<DuplicateUserException>(act);
    }

    [Fact]
    public async Task RegisterUser_ShouldThrow_IfRoleInvalid()
    {
        var service = CreateService();

        Func<Task> act = () => service.RegisterUserAsync(
            "EMP888",
            "valid@agdata.com",
            "Sankalp",
            "Chakre",
            "InvalidRole",
            "User@123"
        );

        await Assert.ThrowsAsync<ArgumentException>(act);
    }

    [Fact]
    public async Task GetUserAccountAsync_ShouldReturnNull_IfUserNotFound()
    {
        var service = CreateService();

        var result = await service.GetUserAccountAsync(Guid.NewGuid());

        Assert.Null(result);
    }
}
