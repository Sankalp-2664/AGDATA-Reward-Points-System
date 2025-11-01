using Application.Services;
using Domain.Exceptions;
using Domain.Entities.User;
using Domain.Enums;
using Infrastructure.Persistence.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Tests.Application.Tests.Services
{
    public class UserServiceTests
    {
        [Fact]
        public async Task RegisterUser_ShouldCreateUserAndAccount()
        {
            // Arrange
            var userRepo = new InMemoryUserRepository();
            var accountRepo = new InMemoryUserAccountRepository();
            var service = new UserService(userRepo, accountRepo);

            // Act
            var user = await service.RegisterUserAsync("EMP001", "sankalp@agdata.com", "sankalp", "chakre",  UserRole.User);

            // Assert
            Assert.NotNull(user);
            Assert.Equal("sankalp@agdata.com", user.Email.Value);

            var account = await accountRepo.GetByUserIdAsync(user.Id);
            Assert.NotNull(account);
            Assert.Equal(0, account!.RewardBalance);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public async Task RegisterUser_ShouldThrow_IfEmployeeIdIsNullOrEmpty(string employeeId)
        {
            var service = new UserService(new InMemoryUserRepository(), new InMemoryUserAccountRepository());

            await Assert.ThrowsAsync<ArgumentException>(() =>
                service.RegisterUserAsync(employeeId, "user@agdata.com", "Sankalp", "C", UserRole.User));
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("invalidemail")]
        public async Task RegisterUser_ShouldThrow_IfEmailIsNullOrInvalid(string email)
        {
            var service = new UserService(new InMemoryUserRepository(), new InMemoryUserAccountRepository());

            await Assert.ThrowsAsync<ArgumentException>(() =>
                service.RegisterUserAsync("EMP123", email, "Sankalp", "C", UserRole.User));
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public async Task RegisterUser_ShouldThrow_IfFirstNameIsNullOrEmpty(string firstName)
        {
            var service = new UserService(new InMemoryUserRepository(), new InMemoryUserAccountRepository());

            await Assert.ThrowsAsync<ArgumentNullException>(() =>
                service.RegisterUserAsync("EMP123", "user@agdata.com", firstName, "C",  UserRole.User));
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public async Task RegisterUser_ShouldThrow_IfLastNameIsNullOrEmpty(string lastName)
        {
            var service = new UserService(new InMemoryUserRepository(), new InMemoryUserAccountRepository());

            await Assert.ThrowsAsync<ArgumentNullException>(() =>
                service.RegisterUserAsync("EMP123", "user@agdata.com", "Sankalp", lastName,  UserRole.User));
        }

        [Theory]
        [InlineData("user@gmail.com")]
        [InlineData("user@outlook.com")]
        [InlineData("user@agdata.co")] // typo in domain
        public async Task RegisterUser_ShouldThrow_IfEmailIsNotAgdata(string email)
        {
            var service = new UserService(new InMemoryUserRepository(), new InMemoryUserAccountRepository());

            await Assert.ThrowsAsync<ArgumentException>(() =>
                service.RegisterUserAsync("EMP123", email, "Sankalp", "C",  UserRole.User));
        }

        [Fact]
        public async Task RegisterUser_ShouldThrow_IfEmployeeIdAlreadyExists()
        {
            var userRepo = new InMemoryUserRepository();
            var accountRepo = new InMemoryUserAccountRepository();
            var service = new UserService(userRepo, accountRepo);

            // First registration
            await service.RegisterUserAsync("EMP001", "first@agdata.com", "First", "Name",  UserRole.User);

            // Second registration with same employee ID
            await Assert.ThrowsAsync<DuplicateUserException>(() =>
                service.RegisterUserAsync("EMP001", "second@agdata.com", "Second", "Name",  UserRole.User));
        }

        [Fact]
        public async Task RegisterUser_ShouldThrow_IfEmailAlreadyExists()
        {
            var userRepo = new InMemoryUserRepository();
            var accountRepo = new InMemoryUserAccountRepository();
            var service = new UserService(userRepo, accountRepo);

            // First registration
            await service.RegisterUserAsync("EMP001", "duplicate@agdata.com", "Alice", "Smith",  UserRole.User);

            // Second registration with same email
            await Assert.ThrowsAsync<DuplicateUserException>(() =>
                service.RegisterUserAsync("EMP002", "duplicate@agdata.com", "Bob", "Brown",  UserRole.User));
        }

        [Fact]
        public async Task RegisterUser_ShouldTrimEmail_AndValidateCorrectly()
        {
            var userRepo = new InMemoryUserRepository();
            var accountRepo = new InMemoryUserAccountRepository();
            var service = new UserService(userRepo, accountRepo);

            var emailWithWhitespace = "  user@agdata.com  ";

            var user = await service.RegisterUserAsync("EMP999", emailWithWhitespace, "Sankalp", "C",  UserRole.User);

            Assert.Equal("user@agdata.com", user.Email.Value);
        }

        [Fact]
        public async Task RegisterUser_ShouldSucceed_WithVeryLongNames()
        {
            var userRepo = new InMemoryUserRepository();
            var accountRepo = new InMemoryUserAccountRepository();
            var service = new UserService(userRepo, accountRepo);

            var longFirstName = new string('A', 1000);
            var longLastName = new string('Z', 1000);

            var user = await service.RegisterUserAsync("EMP_LONG", "longname@agdata.com", longFirstName, longLastName,  UserRole.User);

            Assert.NotNull(user);
            Assert.Equal(longFirstName, user.FirstName);
            Assert.Equal(longLastName, user.LastName);
        }

        [Fact]
        public async Task RegisterUser_ShouldAccept_UppercaseEmail()
        {
            var userRepo = new InMemoryUserRepository();
            var accountRepo = new InMemoryUserAccountRepository();
            var service = new UserService(userRepo, accountRepo);

            var email = "SANKALP.CHAKRE@AGDATA.COM";

            var user = await service.RegisterUserAsync("EMP1234", email, "Sankalp", "Chakre",  UserRole.User);

            Assert.Equal("SANKALP.CHAKRE@AGDATA.COM", user.Email.Value);
        }

        [Fact]
        public async Task GetUserAccountAsync_ShouldReturnNull_IfUserNotFound()
        {
            var userRepo = new InMemoryUserRepository();
            var accountRepo = new InMemoryUserAccountRepository();
            var service = new UserService(userRepo, accountRepo);

            var result = await service.GetUserAccountAsync(Guid.NewGuid());

            Assert.Null(result);
        }

        [Fact]
        public async Task RegisterUser_ShouldThrow_IfEmailMissingAtSymbol()
        {
            var service = new UserService(new InMemoryUserRepository(), new InMemoryUserAccountRepository());

            var invalidEmail = "invalidemail.agdata.com";

            await Assert.ThrowsAsync<ArgumentException>(() =>
                service.RegisterUserAsync("EMP500", invalidEmail, "Sankalp", "C",  UserRole.User));
        }

        [Fact]
        public async Task RegisterUser_ShouldThrow_IfUserRoleInvalid()
        {
            var userRepo = new InMemoryUserRepository();
            var accountRepo = new InMemoryUserAccountRepository();
            var service = new UserService(userRepo, accountRepo);

            var invalidRole = ( UserRole)999;

            await Assert.ThrowsAsync<ArgumentException>(() =>
                service.RegisterUserAsync("EMP888", "valid@agdata.com", "Sankalp", "Chakre", invalidRole));
        }

        [Fact]
        public async Task GetUserAccountAsync_ShouldReturnCorrectAccount()
        {
            var userRepo = new InMemoryUserRepository();
            var accountRepo = new InMemoryUserAccountRepository();
            var service = new UserService(userRepo, accountRepo);

            var user = await service.RegisterUserAsync("EMP777", "sankalp@agdata.com", "Sankalp", "Chakre",  UserRole.User);
            var account = await service.GetUserAccountAsync(user.Id);

            Assert.NotNull(account);
            Assert.Equal(user.Id, account!.UserId);
        }

        public class NullReturningAccountRepo : InMemoryUserAccountRepository
        {
            public override Task<UserAccount?> GetByUserIdAsync(Guid userId) => Task.FromResult<UserAccount?>(null);
        }

        [Fact]
        public async Task GetUserAccountAsync_ShouldReturnNull_WhenRepositoryReturnsNull()
        {
            var userRepo = new InMemoryUserRepository();
            var accountRepo = new NullReturningAccountRepo();
            var service = new UserService(userRepo, accountRepo);

            var result = await service.GetUserAccountAsync(Guid.NewGuid());

            Assert.Null(result);
        }

    }
}
