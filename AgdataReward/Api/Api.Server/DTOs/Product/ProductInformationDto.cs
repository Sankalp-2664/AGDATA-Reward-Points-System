using Domain.Entities.Product;

namespace Api.Server.DTOs.Product;

public class ProductInformationDto
{
    public Guid Id { get; set; }
    public string SKU { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public Guid RewardPointsId { get; set; }

    public static ProductInformationDto FromDomain(ProductInformation entity)
    {
        return new ProductInformationDto
        {
            Id = entity.Id,
            SKU = entity.SKU?.Value ?? string.Empty, 
            Name = entity.Name,
            RewardPointsId = entity.RewardPointsId
        };
    }
}
