using System.ComponentModel.DataAnnotations;

namespace Api.Server.DTOs.Event;

public class EventDefinitionUpdateDto
{
    [Required]
    public Guid Id { get; set; }

    [StringLength(100)]
    public string? Code { get; set; }

    [StringLength(200)]
    public string? Title { get; set; }
}
