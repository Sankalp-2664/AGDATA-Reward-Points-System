using Domain.Entities.User;
using Domain.Enums;
using Domain.ValueObjects;
using FluentAssertions;

namespace Tests.Domain.Tests.Entities;

public class UserProfileTests
{
    [Fact]
    public void Constructor_Should_Throw_When_EmployeeId_Is_Null()
    {
        // Arrange
        var id = Guid.NewGuid();
        var email = new Email("test@agdata.com");
        var firstName = "John";
        var lastName = "Doe";
        var role = UserRole.User;

        // Act
        Action act = () => new UserProfile(
            id,
            null!,
            email,
            firstName,
            lastName,
            role
        );

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .WithMessage("*employeeId*");
    }

    [Fact]
    public void Constructor_Should_Throw_When_Email_Is_Null()
    {
        // Arrange
        var id = Guid.NewGuid();
        var employeeId = new EmployeeId("EMP001");
        var firstName = "John";
        var lastName = "Doe";
        var role = UserRole.User;

        // Act
        Action act = () => new UserProfile(
            id,
            employeeId,
            null!,
            firstName,
            lastName,
            role
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
        var role = UserRole.User;

        // Act
        Action act = () => new UserProfile(
            Guid.NewGuid(),
            employeeId,
            email,
            "",
            lastName,
            role
        );

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .WithMessage("*First Name is required*");
    }

    [Fact]
    public void Constructor_Should_Throw_When_LastName_Is_Empty()
    {
        // Arrange
        var employeeId = new EmployeeId("EMP001");
        var email = new Email("test@agdata.com");
        var firstName = "John";
        var role = UserRole.User;

        // Act
        Action act = () => new UserProfile(
            Guid.NewGuid(),
            employeeId,
            email,
            firstName,
            "",
            role
        );

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .WithMessage("*Last Name is required*");
    }

    [Fact]
    public void Constructor_Should_Initialize_Values_Correctly()
    {
        // Arrange
        var id = Guid.NewGuid();
        var employeeId = new EmployeeId("EMP001");
        var email = new Email("test@agdata.com");
        var firstName = "John";
        var lastName = "Doe";
        var role = UserRole.Admin;

        // Act
        var user = new UserProfile(id, employeeId, email, firstName, lastName, role);

        // Assert
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
        // Arrange
        var employeeId = new EmployeeId("EMP002");
        var email = new Email("test2@agdata.com");

        // Act
        var user = new UserProfile(Guid.Empty, employeeId, email, "Alice", "Smith", UserRole.User);

        // Assert
        user.Id.Should().NotBe(Guid.Empty);
    }
}