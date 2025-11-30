using Domain.Enums;
using Domain.ValueObjects;

namespace Domain.Entities.User
{
    /// <summary>
    /// Aggregate root representing a user profile in the reward system.
    /// </summary>
    public class UserProfile
    {
        /// <summary>
        /// Primary key.
        /// </summary>
        public Guid Id { get; private set; }

        /// <summary>
        /// Employee identifier (value object).
        /// </summary>
        public EmployeeId EmployeeId { get; private set; } = null!;

        /// <summary>
        /// First name of the user.
        /// </summary>
        public string FirstName { get; private set; } = null!;

        /// <summary>
        /// Last name of the user.
        /// </summary>
        public string LastName { get; private set; } = null!;

        /// <summary>
        /// Email value object.
        /// </summary>
        public Email Email { get; private set; } = null!;

        /// <summary>
        /// Role of the user.
        /// </summary>
        public UserRole Role { get; private set; }

        /// <summary>
        /// Navigation property to the user's account.
        /// </summary>
        public virtual UserAccount? Account { get; private set; }

        protected UserProfile() { } // For EF Core

        public UserProfile(EmployeeId employeeId, Email email, string firstName, string lastName, UserRole role)
        {
            Id = Guid.NewGuid();

            EmployeeId = employeeId ?? throw new ArgumentNullException(nameof(employeeId));
            Email = email ?? throw new ArgumentNullException(nameof(email));

            if (string.IsNullOrWhiteSpace(firstName))
                throw new ArgumentException("First name is required.", nameof(firstName));
            if (string.IsNullOrWhiteSpace(lastName))
                throw new ArgumentException("Last name is required.", nameof(lastName));

            FirstName = firstName.Trim();
            LastName = lastName.Trim();
            Role = role;
        }

        public void AttachAccount(UserAccount account)
        {
            if (account == null) throw new ArgumentNullException(nameof(account));
            if (account.UserId != Id) throw new ArgumentException("Account UserId must match UserProfile Id.", nameof(account));

            Account = account;
        }

        /// <summary>
        /// Update basic profile information.
        /// </summary>
        public void UpdateName(string firstName, string lastName)
        {
            if (string.IsNullOrWhiteSpace(firstName)) throw new ArgumentException("First name is required.", nameof(firstName));
            if (string.IsNullOrWhiteSpace(lastName)) throw new ArgumentException("Last name is required.", nameof(lastName));

            FirstName = firstName.Trim();
            LastName = lastName.Trim();
        }
    }
}
