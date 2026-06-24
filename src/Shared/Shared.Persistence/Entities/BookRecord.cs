namespace Shared.Persistence.Entities;

public sealed class BookRecord
{
    public string Id { get; set; } = null!;
    public string Title { get; set; } = null!;
    public string AuthorId { get; set; } = null!;
    public string Isbn { get; set; } = null!;
    public int PublishedYear { get; set; }
}
