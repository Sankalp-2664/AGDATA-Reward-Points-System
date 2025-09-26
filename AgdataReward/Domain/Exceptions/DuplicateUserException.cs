using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Exceptions
{
    public sealed class DuplicateUserException : DomainException
    {
        public DuplicateUserException(string emailOrId)
            : base($"A user with identifier '{emailOrId}' already exists.") { }
    }
}
