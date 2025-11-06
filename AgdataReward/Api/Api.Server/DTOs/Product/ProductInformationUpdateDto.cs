using System.ComponentModel.DataAnnotations;

namespace Api.Server.DTOs.Product;

public class ProductInformationUpdateDto
{
    [Required]
    public Guid Id { get; set; }

    [StringLength(50)]
    public string? SKU { get; set; }

    [StringLength(200)]
    public string? Name { get; set; }

    public Guid? RewardPointsId { get; set; }
}
