using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Services;
using Domain.Entities;
using Infrastructure.Persistence.Repositories;
using Xunit;

namespace Tests.Application.Tests
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
            var user = await service.RegisterUserAsync("EMP001", "sankalp@agdata.com", "sankalp", "chakre");

            // Assert
            Assert.NotNull(user);
            Assert.Equal("sankalp@agdata.com", user.Email);

            var account = await accountRepo.GetByUserIdAsync(user.Id);
            Assert.NotNull(account);
            Assert.Equal(0, account!.RewardBalance);
        }
    }
}
