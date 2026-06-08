using Books.Domain.Entities;
using Books.Domain.Repositories;
using Shared.BuildingBlocks.Persistence;

namespace Books.Infrastructure.Persistence;

public sealed class JsonBookRepository : JsonFileRepository<Book, string>, IBookRepository
{
    public JsonBookRepository(string filePath) : base(filePath)
    {
    }

    protected override string GetKey(Book entity) => entity.Id;

    protected override Book WithKey(Book entity, string key) =>
        new()
        {
            Id = key,
            Title = entity.Title,
            AuthorId = entity.AuthorId,
            Isbn = entity.Isbn,
            PublishedYear = entity.PublishedYear
        };

    public Task<IReadOnlyList<Book>> GetAllAsync(CancellationToken cancellationToken = default) =>
        base.GetAllAsync(cancellationToken);

    public Task<Book?> GetByIdAsync(string id, CancellationToken cancellationToken = default) =>
        base.GetByIdAsync(id, cancellationToken);

    public Task<Book> AddAsync(Book book, CancellationToken cancellationToken = default) =>
        base.AddAsync(book, cancellationToken);

    public Task<Book?> UpdateAsync(string id, Book book, CancellationToken cancellationToken = default) =>
        base.UpdateAsync(
            id,
            _ => new Book
            {
                Id = id,
                Title = book.Title,
                AuthorId = book.AuthorId,
                Isbn = book.Isbn,
                PublishedYear = book.PublishedYear
            },
            cancellationToken);

    public Task<bool> DeleteAsync(string id, CancellationToken cancellationToken = default) =>
        base.DeleteAsync(id, cancellationToken);
}
