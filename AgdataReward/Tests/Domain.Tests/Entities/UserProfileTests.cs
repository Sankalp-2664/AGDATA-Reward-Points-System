using Domain.Entities.User;
using Domain.Enums;
using Domain.ValueObjects;
using FluentAssertions;
using System;
using Xunit;

namespace Tests.Domain.Tests.Entities
{
    public class UserProfileTests
    {
        [Fact]
        public void Constructor_Should_Throw_When_EmployeeId_Is_Null()
        {
            Action act = () => new UserProfile(
                Guid.NewGuid(),
                null!,
                new Email("test@agdata.com"),
                "John",
                "Doe",
                UserRole.User
            );

            act.Should().Throw<ArgumentNullException>()
                .WithMessage("*employeeId*");
        }

        [Fact]
        public void Constructor_Should_Throw_When_Email_Is_Null()
        {
            var employeeId = new EmployeeId("EMP001");
            Action act = () => new UserProfile(
                Guid.NewGuid(),
                employeeId,
                null!,
                "John",
                "Doe",
                UserRole.User
            );

            act.Should().Throw<ArgumentNullException>()
                .WithMessage("*email*");
        }

        [Fact]
        public void Constructor_Should_Throw_When_FirstName_Is_Empty()
        {
            var employeeId = new EmployeeId("EMP001");
            var email = new Email("test@agdata.com");

            Action act = () => new UserProfile(
                Guid.NewGuid(),
                employeeId,
                email,
                "",
                "Doe",
                UserRole.User
            );

            act.Should().Throw<ArgumentNullException>()
                .WithMessage("*First Name is required*");
        }

        [Fact]
        public void Constructor_Should_Throw_When_LastName_Is_Empty()
        {
            var employeeId = new EmployeeId("EMP001");
            var email = new Email("test@agdata.com");

            Action act = () => new UserProfile(
                Guid.NewGuid(),
                employeeId,
                email,
                "John",
                "",
                UserRole.User
            );

            act.Should().Throw<ArgumentNullException>()
                .WithMessage("*Last Name is required*");
        }

        [Fact]
        public void Constructor_Should_Initialize_Values_Correctly()
        {
            var id = Guid.NewGuid();
            var employeeId = new EmployeeId("EMP001");
            var email = new Email("test@agdata.com");
            var firstName = "John";
            var lastName = "Doe";
            var role = UserRole.Admin;

            var user = new UserProfile(id, employeeId, email, firstName, lastName, role);

            user.Id.Should().Be(id);
            user.EmployeeId.Should().Be(employeeId);
            user.Email.Should().Be(email);
            user.FirstName.Should().Be(firstName);
            user.LastName.Should().Be(lastName);
            user.Role.Should().Be(role);
            user.Account.Should().BeNull();
        }

        [Fact]
        public void Constructor_Should_Generate_New_Id_When_Empty()
        {
            var employeeId = new EmployeeId("EMP002");
            var email = new Email("test2@agdata.com");

            var user = new UserProfile(Guid.Empty, employeeId, email, "Alice", "Smith", UserRole.User);

            user.Id.Should().NotBe(Guid.Empty);
        }
    }
}
