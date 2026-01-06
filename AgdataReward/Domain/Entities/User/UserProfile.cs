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
        /// Navigation property to the user's account.
        /// </summary>
        public virtual UserAccount? Account { get; private set; }

        /// <summary>
        /// Roles assigned to the user.
        /// </summary>
        private readonly List<UserRole> _roles = new();
        public IReadOnlyCollection<UserRole> Roles => _roles.AsReadOnly();

        protected UserProfile() { } // For EF Core

        public UserProfile(EmployeeId employeeId, Email email, string firstName, string lastName)
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
        }

        public void AttachAccount(UserAccount account) 
        {
            if (account == null) 
                throw new ArgumentNullException(nameof(account)); 
            
            if (account.UserId != Id) 
                throw new ArgumentException("Account UserId must match UserProfile Id.", 
                    nameof(account)); 

            Account = account; 
        }

        /// <summary>
        /// Assigns a role to the user.
        /// </summary>
        public void AssignRole(Role role)
        {
            if (role == null)
                throw new ArgumentNullException(nameof(role));

            if (_roles.Any(r => r.RoleId == role.Id))
                throw new InvalidOperationException("User already has this role.");

            _roles.Add(new UserRole(Id, role.Id));
        }

        /// <summary>
        /// Removes a role from the user.
        /// </summary>
        public void RemoveRole(Role role)
        {
            if (role == null)
                throw new ArgumentNullException(nameof(role));

            var existing = _roles.FirstOrDefault(r => r.RoleId == role.Id);
            if (existing == null)
                throw new InvalidOperationException("User does not have this role.");

            _roles.Remove(existing);
        }

        /// <summary>
        /// Update basic profile information.
        /// </summary>
        public void UpdateName(string firstName, string lastName)
        {
            if (string.IsNullOrWhiteSpace(firstName))
                throw new ArgumentException("First name is required.", nameof(firstName));
            if (string.IsNullOrWhiteSpace(lastName))
                throw new ArgumentException("Last name is required.", nameof(lastName));

            FirstName = firstName.Trim();
            LastName = lastName.Trim();
        }
    }
}
