using System.ComponentModel.DataAnnotations;

namespace Api.Server.DTOs.User;

public class UserAccountOperationDto
{
    [Required]
    public Guid UserId { get; set; }

    public int? Points { get; set; } // For credit/debit operations

    public string? Action { get; set; } 
}
