using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Exceptions
{
    public sealed class InsufficientPointsException : DomainException
    {
        public InsufficientPointsException(int available, int required)
            : base($"Insufficient points. Available: {available}, Required: {required}.") { }
    }
}
