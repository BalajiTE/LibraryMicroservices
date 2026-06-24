namespace Auth.Application.DTOs;

public sealed record UserDto(string Id, string Username, string Email, bool IsActive, IReadOnlyList<string> Roles);

public sealed record CreateUserRequest(string Username, string Email, string Password, IReadOnlyList<string> RoleNames);

public sealed record UpdateUserRequest(string Email, bool IsActive, IReadOnlyList<string> RoleNames);

public sealed record RoleDto(string Id, string Name, string? Description);

public sealed record CreateRoleRequest(string Name, string? Description);

public sealed record UpdateRoleRequest(string Name, string? Description);

public sealed record LoginRequest(string Username, string Password);

public sealed record RegisterRequest(string Username, string Email, string Password);

public sealed record TokenResponse(string AccessToken, string TokenType, int ExpiresInMinutes, UserDto User);
