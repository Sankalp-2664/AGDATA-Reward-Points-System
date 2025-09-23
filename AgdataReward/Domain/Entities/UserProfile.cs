using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Enums;

namespace Domain.Entities
{
    public class UserProfile
    {
        public Guid Id { get; private set; }
        public string EmployeeId { get; private set; } = null!;
        public string FirstName { get; private set; } = null!;
        public string LastName { get; private set; } = null!;
        public string Email { get; private set; } = null!;

        protected UserProfile() { } // For EF Core

        public UserProfile(Guid id, string employeeId, string email, string firstName, string lastName)
        {
            Id = id == Guid.Empty ? Guid.NewGuid() : id;
            EmployeeId = employeeId ?? throw new ArgumentNullException(nameof(employeeId));
            Email = email ?? throw new ArgumentNullException(nameof(email));
            FirstName = firstName ?? throw new ArgumentNullException(nameof(firstName));
            LastName = lastName ?? throw new ArgumentNullException( nameof(lastName));
        }

    }

}
