using Application.Interfaces;
using Api.Server.DTOs.Auth;
using Microsoft.AspNetCore.Mvc;
using System.Threading;
using System.Threading.Tasks;

namespace Api.Server.Controllers;

/// <summary>
/// Handles authentication-related operations such as user login.
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
            ExpiresAt = result.ExpiresAt
        });
    }
}
