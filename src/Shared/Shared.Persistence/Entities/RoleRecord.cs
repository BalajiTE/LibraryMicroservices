namespace Shared.Persistence.Entities;

public sealed class RoleRecord
{
    public string Id { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
}
