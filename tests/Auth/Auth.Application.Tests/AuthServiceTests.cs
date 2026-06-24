using Auth.Application.DTOs;
using Auth.Application.Services;
using Auth.Infrastructure.Persistence;
using Auth.Infrastructure.Security;
using Microsoft.Extensions.Options;
using Shared.Auth;

namespace Auth.Application.Tests;

public sealed class AuthServiceTests
{
    [Fact]
    public async Task LoginAsync_WithValidCredentials_ReturnsToken()
    {
        var usersPath = Shared.Testing.TempJsonFile.CreateCopy("users.json");
        var rolesPath = Shared.Testing.TempJsonFile.CreateCopy("roles.json");
        var userRolesPath = Shared.Testing.TempJsonFile.CreateCopy("userRoles.json");

        var roleRepository = new JsonRoleRepository(rolesPath);
        var userRepository = new JsonUserRepository(usersPath);
        var userRoleRepository = new JsonUserRoleRepository(userRolesPath, roleRepository);
        var passwordHasher = new PasswordHasherService();
        var jwtOptions = Options.Create(new JwtOptions());
        var tokenGenerator = new JwtTokenGenerator(jwtOptions);
        var authService = new AuthService(
            userRepository,
            userRoleRepository,
            roleRepository,
            passwordHasher,
            tokenGenerator,
            jwtOptions);

        var response = await authService.LoginAsync(new LoginRequest("admin", "Password123!"));

        Assert.NotNull(response);
        Assert.False(string.IsNullOrWhiteSpace(response!.AccessToken));
        Assert.Contains("Admin", response.User.Roles);
    }

    [Fact]
    public async Task LoginAsync_WithInvalidPassword_ReturnsNull()
    {
        var usersPath = Shared.Testing.TempJsonFile.CreateCopy("users.json");
        var rolesPath = Shared.Testing.TempJsonFile.CreateCopy("roles.json");
        var userRolesPath = Shared.Testing.TempJsonFile.CreateCopy("userRoles.json");

        var roleRepository = new JsonRoleRepository(rolesPath);
        var userRepository = new JsonUserRepository(usersPath);
        var userRoleRepository = new JsonUserRoleRepository(userRolesPath, roleRepository);
        var authService = new AuthService(
            userRepository,
            userRoleRepository,
            roleRepository,
            new PasswordHasherService(),
            new JwtTokenGenerator(Options.Create(new JwtOptions())),
            Options.Create(new JwtOptions()));

        var response = await authService.LoginAsync(new LoginRequest("admin", "wrong-password"));

        Assert.Null(response);
    }
}
