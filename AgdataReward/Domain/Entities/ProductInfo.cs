using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public sealed class ProductInfo
{
    public Guid Id { get; }
    public string SKU { get; }
    public string Name { get; }
    public Guid RewardPointsId { get; }

    public ProductInfo(Guid id, string sku, string name, Guid rewardPointsId)
    {
        Id = id;
        SKU = sku;
        Name = name;
        RewardPointsId = rewardPointsId;
    }
}