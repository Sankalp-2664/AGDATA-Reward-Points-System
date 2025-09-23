using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Enums
{
    public enum RedemptionStatus
    {
        Pending,    // User has requested
        Approved,   // Admin approved, ready to process
        Rejected,   // Admin rejected
        Completed,  // Fulfilled
        Cancelled   // User cancelled
    }
}
