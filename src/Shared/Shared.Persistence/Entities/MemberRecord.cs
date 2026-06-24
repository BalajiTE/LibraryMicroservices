namespace Shared.Persistence.Entities;

public sealed class MemberRecord
{
    public string Id { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string? Email { get; set; }
}
