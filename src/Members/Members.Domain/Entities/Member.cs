namespace Members.Domain.Entities;

public sealed class Member
{
    public required string Id { get; init; }
    public required string Name { get; init; }
    public string? Email { get; init; }
}
