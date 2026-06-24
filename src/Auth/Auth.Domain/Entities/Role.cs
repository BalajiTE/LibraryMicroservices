namespace Auth.Domain.Entities;

public sealed class Role
{
    public required string Id { get; init; }
    public required string Name { get; init; }
    public string? Description { get; init; }
}
