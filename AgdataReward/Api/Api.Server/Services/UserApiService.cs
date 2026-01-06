using Api.Server.DTOs.User;
using Application.Interfaces;
using AutoMapper;
using Domain.Enums;

namespace Api.Server.Services;

/// <summary>
/// API-facing user service that coordinates domain services and mapping,
/// so controllers don't need to deal with domain entities or AutoMapper.
/// </summary>
public interface IUserApiService
{
    Task<UserProfileDto?> GetUserByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<UserProfileDto> RegisterUserAsync(UserProfileCreateDto dto, CancellationToken cancellationToken = default);
    Task<UserAccountDto?> GetUserAccountAsync(Guid id, CancellationToken cancellationToken = default);
}

public class UserApiService(
    IUserService userService,
    IMapper mapper) : IUserApiService
{
    private readonly IUserService _userService = userService;
    private readonly IMapper _mapper = mapper;

    public async Task<UserProfileDto?> GetUserByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var user = await _userService.GetUserByIdAsync(id);

        return user is null
            ? null
            : _mapper.Map<UserProfileDto>(user);
    }

    public async Task<UserProfileDto> RegisterUserAsync(
        UserProfileCreateDto dto,
        CancellationToken cancellationToken = default)
    {
        // Move role parsing here (keep controller clean)
        var role = Enum.Parse<UserRole>(dto.Role, ignoreCase: true);

        var user = await _userService.RegisterUserAsync(
            dto.EmployeeId,
            dto.Email,
            dto.FirstName,
            dto.LastName,
            role,
            dto.Password);

        return _mapper.Map<UserProfileDto>(user);
    }

    public async Task<UserAccountDto?> GetUserAccountAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var account = await _userService.GetUserAccountAsync(id);

        return account is null
            ? null
            : _mapper.Map<UserAccountDto>(account);
    }
}
