using Books.Domain.Entities;
using Books.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using Shared.Persistence;
using Shared.Persistence.Entities;

namespace Books.Infrastructure.Persistence;

public sealed class SqlBookRepository(LibraryDbContext context) : IBookRepository
{
    public async Task<IReadOnlyList<Book>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var books = await context.Books
            .AsNoTracking()
            .OrderBy(book => book.Title)
            .ToListAsync(cancellationToken);

        return books.Select(Map).ToList();
    }

    public async Task<Book?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        var book = await context.Books
            .AsNoTracking()
            .FirstOrDefaultAsync(record => record.Id == id, cancellationToken);

        return book is null ? null : Map(book);
    }

    public async Task<Book> AddAsync(Book book, CancellationToken cancellationToken = default)
    {
        context.Books.Add(Map(book));
        await context.SaveChangesAsync(cancellationToken);
        return book;
    }

    public async Task<Book?> UpdateAsync(string id, Book book, CancellationToken cancellationToken = default)
    {
        var existing = await context.Books.FirstOrDefaultAsync(record => record.Id == id, cancellationToken);
        if (existing is null)
        {
            return null;
        }

        existing.Title = book.Title;
        existing.AuthorId = book.AuthorId;
        existing.Isbn = book.Isbn;
        existing.PublishedYear = book.PublishedYear;
        await context.SaveChangesAsync(cancellationToken);
        return Map(existing);
    }

    public async Task<bool> DeleteAsync(string id, CancellationToken cancellationToken = default)
    {
        var existing = await context.Books.FirstOrDefaultAsync(record => record.Id == id, cancellationToken);
        if (existing is null)
        {
            return false;
        }

        context.Books.Remove(existing);
        await context.SaveChangesAsync(cancellationToken);
        return true;
    }

    private static Book Map(BookRecord record) =>
        new()
        {
            Id = record.Id,
            Title = record.Title,
            AuthorId = record.AuthorId,
            Isbn = record.Isbn,
            PublishedYear = record.PublishedYear
        };

    private static BookRecord Map(Book book) =>
        new()
        {
            Id = book.Id,
            Title = book.Title,
            AuthorId = book.AuthorId,
            Isbn = book.Isbn,
            PublishedYear = book.PublishedYear
        };
}
