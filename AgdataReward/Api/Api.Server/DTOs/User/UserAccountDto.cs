using System;

namespace Api.Server.DTOs.User;

/// <summary>
/// Represents a user's reward account returned by the API.
/// </summary>
public sealed class UserAccountDto
{
    /// <summary>Unique identifier of the account.</summary>
    public Guid Id { get; set; }

    /// <summary>Current reward points balance.</summary>
    public int RewardBalance { get; set; }

    /// <summary>Account status as string (e.g. "Active").</summary>
    public string Status { get; set; } = string.Empty;
}
