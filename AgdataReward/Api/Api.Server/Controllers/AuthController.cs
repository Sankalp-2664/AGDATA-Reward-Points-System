using Application.Interfaces;
using Api.Server.DTOs.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace Api.Server.Controllers;

/// <summary>
/// Handles authentication-related operations such as user login and logout.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class AuthController(IAuthService authService) : ControllerBase
{
    private readonly IAuthService _authService = authService;

    /// <summary>
    /// Authenticates a user and returns a JWT access token.
    /// </summary>
    /// <param name="dto">Login credentials containing email and password.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A JWT token and expiration timestamp.</returns>
    /// <remarks>
    /// Sample request:
    ///
    ///     POST /api/auth/login
    ///     {
    ///       "email": "admin@agdata.com",
    ///       "password": "Admin@123"
    ///     }
    ///
    /// </remarks>
    /// <response code="200">Returns the valid JWT token.</response>
    /// <response code="400">If the login request model is invalid.</response>
    /// <response code="401">If credentials are incorrect.</response>
    [HttpPost("login")]
    [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<AuthResponseDto>> Login(
        [FromBody] LoginRequestDto dto,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _authService.LoginAsync(dto.Email, dto.Password, cancellationToken);

        if (!result.Success)
            return Unauthorized(new { message = result.Error });

        return Ok(new AuthResponseDto
        {
            Token = result.Token,
            ExpiresAt = result.ExpiresAt,
            UserId = result.UserId
        });
    }

    /// <summary>
    /// Logs out the authenticated user.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Success message.</returns>
    /// <remarks>
    /// This endpoint requires a valid JWT token in the Authorization header.
    /// The frontend should discard the token after receiving a successful response.
    ///
    /// Sample request:
    ///
    ///     POST /api/auth/logout
    ///     Authorization: Bearer {token}
    ///
    /// </remarks>
    /// <response code="200">User logged out successfully.</response>
    /// <response code="401">If the user is not authenticated.</response>
    [HttpPost("logout")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult> Logout(CancellationToken cancellationToken)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userId))
            return Unauthorized(new { message = "User ID not found in token." });

        var result = await _authService.LogoutAsync(userId, cancellationToken);

        if (!result.Success)
            return BadRequest(new { message = result.Error });

        return Ok(new { message = "User logged out successfully." });
    }
}
