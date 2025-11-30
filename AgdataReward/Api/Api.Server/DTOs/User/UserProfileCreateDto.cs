using System.ComponentModel.DataAnnotations;

public sealed class UserProfileCreateDto
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

    /// <summary>User role name (default = "User").</summary>
    public string Role { get; set; } = "User";

    [Required]
    public string Password { get; set; } = string.Empty;
}
