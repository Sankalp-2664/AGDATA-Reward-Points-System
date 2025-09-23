using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities;

public sealed class RewardTransactionType
{
    public Guid Id { get; }
    public string TypeName { get; }

    public RewardTransactionType(Guid id, string typeName)
    {
        Id = id;
        TypeName = typeName;
    }
}
