using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Exceptions
{
    public sealed class InvalidRedemptionException : DomainException
    {
        public InvalidRedemptionException(string reason)
            : base($"Invalid redemption: {reason}") { }
    }
}
