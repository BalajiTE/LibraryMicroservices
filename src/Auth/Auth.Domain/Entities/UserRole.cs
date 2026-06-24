namespace Auth.Domain.Entities;

public sealed class UserRole
{
    public required string UserId { get; init; }
    public required string RoleId { get; init; }
}
