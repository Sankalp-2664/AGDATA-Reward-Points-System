using System.ComponentModel.DataAnnotations;

namespace Api.Server.DTOs.User;

/// <summary>
/// DTO for updating an existing user profile.
/// </summary>
public sealed class UserProfileUpdateDto
{
    /// <summary>First name of the user.</summary>
    [Required]
    [MaxLength(100)]
    public string FirstName { get; set; } = string.Empty;

    /// <summary>Last name of the user.</summary>
    [Required]
    [MaxLength(100)]
    public string LastName { get; set; } = string.Empty;

    /// <summary>Email address.</summary>
    [Required]
    [EmailAddress]
    [MaxLength(255)]
    public string Email { get; set; } = string.Empty;

    /// <summary>User role (e.g., "User", "Admin").</summary>
    [Required]
    public string Role { get; set; } = string.Empty;

    /// <summary>Account status (e.g., "Active", "Inactive").</summary>
    public string? AccountStatus { get; set; }
}
