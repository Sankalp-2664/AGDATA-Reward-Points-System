using System.ComponentModel.DataAnnotations;

namespace Api.Server.DTOs.User;

public class UserProfileCreateDto
{
    [Required]
    public string EmployeeId { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    public string LastName { get; set; } = string.Empty;

    [Required]
    public string Role { get; set; } = string.Empty; // Admin, User
}
