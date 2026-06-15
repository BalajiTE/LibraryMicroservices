namespace Authors.Domain.Entities;

public sealed class Author
{
    public required string Id { get; init; }
    public required string Name { get; init; }
    public string? Bio { get; init; }
}
