using System;
using System.ComponentModel.DataAnnotations;

namespace Api.Server.DTOs.User;

/// <summary>
/// Request payload for performing operations on a user's reward account
/// (e.g., crediting or debiting points).
/// </summary>
public sealed class UserAccountOperationDto
{
    /// <summary>User identifier whose account is being modified.</summary>
    [Required]
    public Guid UserId { get; set; }

    /// <summary>
    /// Number of points to adjust.
    /// Positive to credit, negative to debit. Nullable if only reading data.
    /// </summary>
    public int? Points { get; set; }

    /// <summary>
    /// Optional action label or description (e.g., "ManualAdjustment", "Bonus").
    /// </summary>
    public string? Action { get; set; }
}
