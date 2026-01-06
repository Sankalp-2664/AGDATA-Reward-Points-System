using System.ComponentModel.DataAnnotations;

namespace Api.Server.DTOs.Event;

public class EventDefinitionCreateDto
{
    [Required]
    [StringLength(100)]
    public string Code { get; set; } = string.Empty;

    [Required]
    [StringLength(200)]
    public string Title { get; set; } = string.Empty;
}
