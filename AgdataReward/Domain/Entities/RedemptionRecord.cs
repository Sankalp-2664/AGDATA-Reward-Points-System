using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities;

public sealed class RedemptionRecord
{
    public Guid Id { get; }
    public Guid UserId { get; }
    public Guid ProductId { get; }

    public RedemptionRecord(Guid id, Guid userId, Guid productId)
    {
        Id = id;
        UserId = userId;
        ProductId = productId;
    }
}
