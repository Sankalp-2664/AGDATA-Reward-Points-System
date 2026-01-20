using Api.Server.DTOs.User;
using Application.Interfaces;
using AutoMapper;

namespace Api.Server.Services;

/// <summary>
/// API-facing user service that coordinates domain services and mapping,
/// so controllers don't need to deal with domain entities or AutoMapper.
/// </summary>
public interface IUserApiService
{
    Task<UserProfileDto?> GetUserByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<UserProfileDto?> GetCurrentUserAsync(string userId, CancellationToken cancellationToken = default);
    Task<UserProfileDto> RegisterUserAsync(UserProfileCreateDto dto, CancellationToken cancellationToken = default);
    Task<UserAccountDto?> GetUserAccountAsync(Guid id, CancellationToken cancellationToken = default);
    Task<List<UserProfileDto>> GetAllUsersAsync(CancellationToken cancellationToken);
    Task<UserProfileDto?> UpdateUserAsync(Guid id, UserProfileUpdateDto dto, CancellationToken cancellationToken = default);

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

    public async Task<UserProfileDto?> GetCurrentUserAsync(
        string userId,
        CancellationToken cancellationToken = default)
    {
        if (Guid.TryParse(userId, out var guid))
        {
            var user = await _userService.GetUserByIdAsync(guid);
            return user is null ? null : _mapper.Map<UserProfileDto>(user);
        }
        return null;
    }

    public async Task<List<UserProfileDto>> GetAllUsersAsync(CancellationToken cancellationToken)
    {
        var users = await _userService.GetAllUsersAsync(cancellationToken);

        return _mapper.Map<List<UserProfileDto>>(users);
    }


    public async Task<UserProfileDto> RegisterUserAsync(
        UserProfileCreateDto dto,
        CancellationToken cancellationToken = default)
    {
        var user = await _userService.RegisterUserAsync(
            dto.EmployeeId,
            dto.Email,
            dto.FirstName,
            dto.LastName,
            dto.Role,
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

    public async Task<UserProfileDto?> UpdateUserAsync(
        Guid id,
        UserProfileUpdateDto dto,
        CancellationToken cancellationToken = default)
    {
        var updatedUser = await _userService.UpdateUserAsync(
            id,
            dto.FirstName,
            dto.LastName,
            dto.Email,
            dto.Role,
            dto.AccountStatus);

        return updatedUser is null
            ? null
            : _mapper.Map<UserProfileDto>(updatedUser);
    }
}
