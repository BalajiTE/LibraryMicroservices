using Auth.Application.DTOs;
using Auth.Application.Services;
using Auth.Domain.Entities;
using Auth.Domain.Repositories;
using Microsoft.Extensions.Options;
using Shared.Auth;

namespace Auth.Application.Services;

public sealed class AuthService(
    IUserRepository userRepository,
    IUserRoleRepository userRoleRepository,
    IRoleRepository roleRepository,
    IPasswordHasher passwordHasher,
    IJwtTokenGenerator jwtTokenGenerator,
    IOptions<JwtOptions> jwtOptions) : IAuthService
{
    public async Task<TokenResponse?> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
        {
            throw new ArgumentException("Username and password are required.");
        }

        var user = await userRepository.GetByUsernameAsync(request.Username.Trim(), cancellationToken);
        if (user is null || !user.IsActive)
        {
            return null;
        }

        if (!passwordHasher.VerifyPassword(request.Password, user.PasswordHash))
        {
            return null;
        }

        return await BuildTokenResponseAsync(user, cancellationToken);
    }

    public async Task<TokenResponse> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default)
    {
        ValidateRegistration(request);

        var existing = await userRepository.GetByUsernameAsync(request.Username.Trim(), cancellationToken);
        if (existing is not null)
        {
            throw new ArgumentException("Username is already taken.");
        }

        var memberRole = await roleRepository.GetByNameAsync(LibraryRoles.Member, cancellationToken)
            ?? throw new InvalidOperationException($"Default role '{LibraryRoles.Member}' was not found.");

        var user = new User
        {
            Id = Guid.NewGuid().ToString("N")[..8],
            Username = request.Username.Trim(),
            Email = request.Email.Trim(),
            PasswordHash = passwordHasher.HashPassword(request.Password),
            IsActive = true
        };

        await userRepository.AddAsync(user, cancellationToken);
        await userRoleRepository.AssignAsync(user.Id, memberRole.Id, cancellationToken);

        return await BuildTokenResponseAsync(user, cancellationToken);
    }

    private async Task<TokenResponse> BuildTokenResponseAsync(User user, CancellationToken cancellationToken)
    {
        var roleNames = await userRoleRepository.GetRoleNamesForUserAsync(user.Id, cancellationToken);
        var token = jwtTokenGenerator.GenerateToken(user.Id, user.Username, roleNames);

        return new TokenResponse(
            token,
            "Bearer",
            jwtOptions.Value.ExpirationMinutes,
            new UserDto(user.Id, user.Username, user.Email, user.IsActive, roleNames));
    }

    private static void ValidateRegistration(RegisterRequest request)
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
}
