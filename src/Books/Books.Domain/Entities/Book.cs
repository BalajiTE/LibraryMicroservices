namespace Books.Domain.Entities;

public sealed class Book
{
    public required string Id { get; init; }
    public required string Title { get; init; }
    public required string AuthorId { get; init; }
    public required string Isbn { get; init; }
    public int PublishedYear { get; init; }
}
