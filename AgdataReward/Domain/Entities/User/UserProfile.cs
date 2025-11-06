using Domain.Enums;
using Domain.ValueObjects;

namespace Domain.Entities.User;

public class UserProfile
{
    public Guid Id { get; private set; } // Primary Key
    public EmployeeId EmployeeId { get; private set; } = null!; // Unique Employee Identifier
    public string FirstName { get; private set; } = null!; 
    public string LastName { get; private set; } = null!;
    public Email Email { get; private set; } = null!;
    public UserRole Role { get; private set; } // User role (e.g., Admin, User)

    public virtual UserAccount? Account { get; private set; } // Navigation property to UserAccount
    protected UserProfile() { } // For EF Core

    public UserProfile(Guid id, EmployeeId employeeId, Email email, string firstName, string lastName, UserRole role)
    {
        Id = id == Guid.Empty ? Guid.NewGuid() : id;
        EmployeeId = employeeId ?? throw new ArgumentNullException(nameof(employeeId));
        Email = email ?? throw new ArgumentNullException(nameof(email));

        FirstName = !string.IsNullOrWhiteSpace(firstName) ? firstName : throw new ArgumentNullException(nameof(firstName), "First Name is required.");
        LastName = !string.IsNullOrWhiteSpace(lastName) ? lastName : throw new ArgumentNullException(nameof(lastName), "Last Name is required.");

        Role = role;
    }

}
