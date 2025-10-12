using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities.Reward;
using Domain.Enums;

namespace Domain.Entities.User
{
    public class UserProfile
    {
        public Guid Id { get; private set; } // Primary Key
        public string EmployeeId { get; private set; } = null!; // Unique Employee Identifier
        public string FirstName { get; private set; } = null!;
        public string LastName { get; private set; } = null!;
        public string Email { get; private set; } = null!;
        public UserRole Role { get; private set; } 

        public virtual UserAccount? Account { get; private set; } // Navigation property to UserAccount
        protected UserProfile() { } // For EF Core

        public UserProfile(Guid id, string employeeId, string email, string firstName, string lastName, UserRole role)
        {
            Id = id == Guid.Empty ? Guid.NewGuid() : id;
            EmployeeId = !string.IsNullOrWhiteSpace(employeeId) ? employeeId : throw new ArgumentNullException(nameof(employeeId), "Employee ID is required.");
            Email = !string.IsNullOrWhiteSpace(email) ? email : throw new ArgumentNullException(nameof(email), "Email is required.");
            FirstName = !string.IsNullOrWhiteSpace(firstName) ? firstName : throw new ArgumentNullException(nameof(firstName), "First Name is required.");
            LastName = !string.IsNullOrWhiteSpace(lastName) ? lastName : throw new ArgumentNullException(nameof(lastName), "Last Name is required.");

            Role = role;
        }

    }

}
