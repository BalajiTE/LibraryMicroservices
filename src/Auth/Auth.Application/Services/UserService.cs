using Auth.Application.DTOs;
using Auth.Domain.Entities;
using Auth.Domain.Repositories;

namespace Auth.Application.Services;

public sealed class UserService(
    IUserRepository userRepository,
    IUserRoleRepository userRoleRepository,
    IRoleRepository roleRepository,
    IPasswordHasher passwordHasher) : IUserService
{
    public async Task<IReadOnlyList<UserDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var users = await userRepository.GetAllAsync(cancellationToken);
        var result = new List<UserDto>();

        foreach (var user in users)
        {
            var roles = await userRoleRepository.GetRoleNamesForUserAsync(user.Id, cancellationToken);
            result.Add(Map(user, roles));
        }

        return result;
    }

    public async Task<UserDto?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        var user = await userRepository.GetByIdAsync(id, cancellationToken);
        if (user is null)
        {
            return null;
        }

        var roles = await userRoleRepository.GetRoleNamesForUserAsync(user.Id, cancellationToken);
        return Map(user, roles);
    }

    public async Task<UserDto> CreateAsync(CreateUserRequest request, CancellationToken cancellationToken = default)
    {
        ValidateCreateRequest(request);

        var existing = await userRepository.GetByUsernameAsync(request.Username.Trim(), cancellationToken);
        if (existing is not null)
        {
            throw new ArgumentException("Username is already taken.");
        }

        var user = new User
        {
            Id = Guid.NewGuid().ToString("N")[..8],
            Username = request.Username.Trim(),
            Email = request.Email.Trim(),
            PasswordHash = passwordHasher.HashPassword(request.Password),
            IsActive = true
        };

        await userRepository.AddAsync(user, cancellationToken);
        await SyncRolesAsync(user.Id, request.RoleNames, cancellationToken);

        var roles = await userRoleRepository.GetRoleNamesForUserAsync(user.Id, cancellationToken);
        return Map(user, roles);
    }

    public async Task<UserDto?> UpdateAsync(string id, UpdateUserRequest request, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.Email))
        {
            throw new ArgumentException("Email is required.");
        }

        var existing = await userRepository.GetByIdAsync(id, cancellationToken);
        if (existing is null)
        {
            return null;
        }

        var updated = await userRepository.UpdateAsync(
            id,
            new User
            {
                Id = id,
                Username = existing.Username,
                Email = request.Email.Trim(),
                PasswordHash = existing.PasswordHash,
                IsActive = request.IsActive
            },
            cancellationToken);

        if (updated is null)
        {
            return null;
        }

        await SyncRolesAsync(id, request.RoleNames, cancellationToken);
        var roles = await userRoleRepository.GetRoleNamesForUserAsync(id, cancellationToken);
        return Map(updated, roles);
    }

    public Task<bool> DeleteAsync(string id, CancellationToken cancellationToken = default) =>
        userRepository.DeleteAsync(id, cancellationToken);

    private async Task SyncRolesAsync(string userId, IReadOnlyList<string> roleNames, CancellationToken cancellationToken)
    {
        var currentRoleIds = await userRoleRepository.GetRoleIdsForUserAsync(userId, cancellationToken);
        var desiredRoleIds = new List<string>();

        foreach (var roleName in roleNames.Distinct(StringComparer.OrdinalIgnoreCase))
        {
            var role = await roleRepository.GetByNameAsync(roleName.Trim(), cancellationToken)
                ?? throw new ArgumentException($"Role '{roleName}' was not found.");
            desiredRoleIds.Add(role.Id);
        }

        foreach (var roleId in currentRoleIds.Where(roleId => !desiredRoleIds.Contains(roleId)))
        {
            await userRoleRepository.RemoveAsync(userId, roleId, cancellationToken);
        }

        foreach (var roleId in desiredRoleIds.Where(roleId => !currentRoleIds.Contains(roleId)))
        {
            await userRoleRepository.AssignAsync(userId, roleId, cancellationToken);
        }
    }

    private static void ValidateCreateRequest(CreateUserRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Username))
        {
            throw new ArgumentException("Username is required.");
        }

        if (string.IsNullOrWhiteSpace(request.Email))
        {
            throw new ArgumentException("Email is required.");
        }

        if (string.IsNullOrWhiteSpace(request.Password) || request.Password.Length < 8)
        {
            throw new ArgumentException("Password must be at least 8 characters.");
        }
    }

    private static UserDto Map(User user, IReadOnlyList<string> roles) =>
        new(user.Id, user.Username, user.Email, user.IsActive, roles);
}
