using System.ComponentModel.DataAnnotations;

namespace Api.Server.DTOs.Product;

public class ProductInformationCreateDto
{
    [Required]
    [StringLength(50)]
    public string SKU { get; set; } = string.Empty;

    [Required]
    [StringLength(200)]
    public string Name { get; set; } = string.Empty;

    [Required]
    public Guid RewardPointsId { get; set; }
}
