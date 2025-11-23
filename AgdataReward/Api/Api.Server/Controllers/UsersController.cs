using Api.Server.DTOs.User;
using Application.Interfaces;
using Domain.Enums;
using Microsoft.AspNetCore.Mvc;

namespace Api.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    // GET: api/users/{id}
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<UserProfileDto>> GetUserById(Guid id)
    {
        var user = await _userService.GetUserByIdAsync(id);
        if (user == null) return NotFound();

        var dto = new UserProfileDto
        {
            Id = user.Id,
            EmployeeId = user.EmployeeId.Value,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email.Value,
            Role = user.Role.ToString(),
            Account = user.Account != null
                ? new UserAccountDto
                {
                    Id = user.Account.Id,
                    RewardBalance = user.Account.RewardBalance,
                    Status = user.Account.Status.ToString()
                }
                : null
        };

        return Ok(dto);
    }

    // POST: api/users
    [HttpPost]
    public async Task<ActionResult<UserProfileDto>> CreateUser(UserProfileCreateDto createDto)
    {
        try
        {
            var user = await _userService.RegisterUserAsync(
                createDto.EmployeeId,
                createDto.Email,
                createDto.FirstName,
                createDto.LastName,
                Enum.Parse<UserRole>(createDto.Role, true)
            );

            var dto = new UserProfileDto
            {
                Id = user.Id,
                EmployeeId = user.EmployeeId.Value,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email.Value,
                Role = user.Role.ToString(),
                Account = user.Account != null
                    ? new UserAccountDto
                    {
                        Id = user.Account.Id,
                        RewardBalance = user.Account.RewardBalance,
                        Status = user.Account.Status.ToString()
                    }
                    : null
            };

            return CreatedAtAction(nameof(GetUserById), new { id = user.Id }, dto);
        }
        catch (Exception ex)
        {
            return BadRequest(new { Message = ex.Message });
        }
    }

    // GET: api/users/{id}/account
    [HttpGet("{id:guid}/account")]
    public async Task<ActionResult<UserAccountDto>> GetUserAccount(Guid id)
    {
        var account = await _userService.GetUserAccountAsync(id);
        if (account == null) return NotFound();

        var dto = new UserAccountDto
        {
            Id = account.Id,
            RewardBalance = account.RewardBalance,
            Status = account.Status.ToString()
        };

        return Ok(dto);
    }
}
