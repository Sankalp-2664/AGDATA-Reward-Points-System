using System;

namespace Api.Server.DTOs.User;

/// <summary>
/// Represents a user profile returned by the API.
/// </summary>
public sealed class UserProfileDto
{
    /// <summary>Unique identifier of the user.</summary>
    public Guid Id { get; set; }

    /// <summary>Employee identifier (business ID).</summary>
    public string EmployeeId { get; set; } = string.Empty;

    /// <summary>First name of the user.</summary>
    public string FirstName { get; set; } = string.Empty;

    /// <summary>Last name of the user.</summary>
    public string LastName { get; set; } = string.Empty;

    /// <summary>Email address.</summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>User role as string (e.g. "Admin").</summary>
    public IEnumerable<string> Roles { get; set; } = [];

    /// <summary>Reward account information for the user, if available.</summary>
    public UserAccountDto? Account { get; set; }
}
