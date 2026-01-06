using Api.Server.DTOs.User;
using Application.Interfaces;
using Api.Server.Services;
using AutoMapper;
using Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Server.Controllers;

/// <summary>
/// Manages user profiles and reward accounts in the reward system.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class UsersController(IUserApiService userApiService, ILogger<UsersController> logger)
    : ControllerBase
{
    private readonly IUserApiService _userApiService = userApiService;
    private readonly ILogger<UsersController> _logger = logger;

    /// <summary>
    /// Gets a user profile by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the user.</param>
    /// <returns>The user profile details.</returns>
    /// <remarks>
    /// Sample request:
    ///
    ///     GET /api/users/{id}
    ///
    /// Sample response:
    ///
    ///     200 OK
    ///     {
    ///       "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    ///       "employeeId": "EMP001",
    ///       "firstName": "Sankalp",
    ///       "lastName": "Chakre",
    ///       "email": "user@agdata.com",
    ///       "role": "User",
    ///       "account": {
    ///         "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    ///         "rewardBalance": 100,
    ///         "status": "Active"
    ///       }
    ///     }
    /// </remarks>
    /// <response code="200">Returns the user profile.</response>
    /// <response code="401">If the caller is not authenticated.</response>
    /// <response code="404">If a user with the given ID was not found.</response>
    [HttpGet("{id:guid}")]
    [Authorize]
    [ProducesResponseType(typeof(UserProfileDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UserProfileDto>> GetUserById(Guid id, CancellationToken cancellationToken)
    {
        var userDto = await _userApiService.GetUserByIdAsync(id, cancellationToken);

        if (userDto is null)
            return NotFound();

        return Ok(userDto);
    }

    /// <summary>
    /// Registers a new user in the reward system.
    /// </summary>
    /// <param name="createDto">User registration payload.</param>
    /// <returns>The newly created user profile.</returns>
    /// <remarks>
    /// Sample request:
    ///
    ///     POST /api/users
    ///     {
    ///       "employeeId": "EMP200",
    ///       "email": "trial3@agdata.com",
    ///       "firstName": "Trial",
    ///       "lastName": "User",
    ///       "role": "User",
    ///       "password": "Str0ngP@ss!"
    ///     }
    ///
    /// Sample response:
    ///
    ///     201 Created
    ///     {
    ///       "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    ///       "employeeId": "EMP200",
    ///       "firstName": "Trial",
    ///       "lastName": "User",
    ///       "email": "trial3@agdata.com",
    ///       "role": "User",
    ///       "account": {
    ///         "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    ///         "rewardBalance": 0,
    ///         "status": "Active"
    ///       }
    ///     }
    ///
    /// </remarks>
    /// <response code="201">Returns the newly created user profile.</response>
    /// <response code="400">If the request body is invalid or the user already exists.</response>
    [HttpPost]
    [AllowAnonymous]
    [ProducesResponseType(typeof(UserProfileDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<UserProfileDto>> CreateUser(
        [FromBody] UserProfileCreateDto createDto,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var createdUserDto = await _userApiService.RegisterUserAsync(createDto, cancellationToken);

            return CreatedAtAction(nameof(GetUserById),
                new { id = createdUserDto.Id },
                createdUserDto);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Error creating user");
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Gets reward account details for a specific user.
    /// </summary>
    /// <param name="id">The unique identifier of the user.</param>
    /// <returns>The reward account information associated with the user.</returns>
    /// <remarks>
    /// Sample request:
    ///
    ///     GET /api/users/{id}/account
    ///
    /// Sample response:
    ///
    ///     200 OK
    ///     {
    ///       "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    ///       "rewardBalance": 150,
    ///       "status": "Active"
    ///     }
    /// </remarks>
    /// <response code="200">Returns the user's reward account.</response>
    /// <response code="401">If the caller is not authenticated.</response>
    /// <response code="404">If the user or account is not found.</response>
    [HttpGet("{id:guid}/account")]
    [Authorize]
    [ProducesResponseType(typeof(UserAccountDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UserAccountDto>> GetUserAccount(
        Guid id,
        CancellationToken cancellationToken)
    {
        var accountDto = await _userApiService.GetUserAccountAsync(id, cancellationToken);

        if (accountDto is null)
            return NotFound();

        return Ok(accountDto);
    }
}
