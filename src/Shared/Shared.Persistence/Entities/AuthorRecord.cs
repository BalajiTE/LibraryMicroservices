namespace Shared.Persistence.Entities;

public sealed class AuthorRecord
{
    public string Id { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string? Bio { get; set; }
}
