using Domain.Entities.User;
using Domain.ValueObjects;
using FluentAssertions;

namespace Tests.Domain.Tests.Entities;

public class UserProfileTests
{
    [Fact]
    public void Constructor_Should_Throw_When_EmployeeId_Is_Null()
    {
        // Arrange
        var email = new Email("test@agdata.com");
        var firstName = "John";
        var lastName = "Doe";

        // Act
        Action act = () => new UserProfile(
            null!,
            email,
            firstName,
            lastName
        );

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .WithMessage("*employeeId*");
    }

    [Fact]
    public void Constructor_Should_Throw_When_Email_Is_Null()
    {
        // Arrange
        var employeeId = new EmployeeId("EMP001");
        var firstName = "John";
        var lastName = "Doe";

        // Act
        Action act = () => new UserProfile(
            employeeId,
            null!,
            firstName,
            lastName
        );

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .WithMessage("*email*");
    }

    [Fact]
    public void Constructor_Should_Throw_When_FirstName_Is_Empty()
    {
        // Arrange
        var employeeId = new EmployeeId("EMP001");
        var email = new Email("test@agdata.com");
        var lastName = "Doe";

        // Act
        Action act = () => new UserProfile(
            employeeId,
            email,
            "",
            lastName
        );

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*First name is required*");
    }

    [Fact]
    public void Constructor_Should_Throw_When_LastName_Is_Empty()
    {
        // Arrange
        var employeeId = new EmployeeId("EMP001");
        var email = new Email("test@agdata.com");
        var firstName = "John";

        // Act
        Action act = () => new UserProfile(
            employeeId,
            email,
            firstName,
            ""
        );

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*Last name is required*");
    }

    [Fact]
    public void Constructor_Should_Initialize_Values_Correctly()
    {
        // Arrange
        var employeeId = new EmployeeId("EMP001");
        var email = new Email("test@agdata.com");
        var firstName = "John";
        var lastName = "Doe";

        // Act
        var user = new UserProfile(employeeId, email, firstName, lastName);

        // Assert
        user.Id.Should().NotBe(Guid.Empty);
        user.EmployeeId.Should().Be(employeeId);
        user.Email.Should().Be(email);
        user.FirstName.Should().Be(firstName);
        user.LastName.Should().Be(lastName);
        user.Account.Should().BeNull();
        user.Roles.Should().BeEmpty(); // No roles assigned yet
    }

    [Fact]
    public void AssignRole_Should_AddRoleToUser()
    {
        // Arrange
        var employeeId = new EmployeeId("EMP003");
        var email = new Email("test3@agdata.com");
        var user = new UserProfile(employeeId, email, "Alice", "Smith");

        var role = new Role("USER");

        // Act
        user.AssignRole(role);

        // Assert
        user.Roles.Should().HaveCount(1);
        user.Roles.First().RoleId.Should().Be(role.Id);
        user.Roles.First().UserId.Should().Be(user.Id);
    }

    [Fact]
    public void AssignRole_Should_Throw_WhenRoleAlreadyAssigned()
    {
        // Arrange
        var employeeId = new EmployeeId("EMP004");
        var email = new Email("test4@agdata.com");
        var user = new UserProfile(employeeId, email, "Bob", "Brown");

        var role = new Role("USER");
        user.AssignRole(role);

        // Act
        Action act = () => user.AssignRole(role);

        // Assert
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("User already has this role.");
    }
}
