using Books.Application.DTOs;
using Books.Domain.Entities;
using Books.Domain.Repositories;

namespace Books.Application.Services;

public sealed class BookService(IBookRepository repository) : IBookService
{
    public async Task<IReadOnlyList<BookDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var books = await repository.GetAllAsync(cancellationToken);
        return books.Select(Map).ToList();
    }

    public async Task<BookDto?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        var book = await repository.GetByIdAsync(id, cancellationToken);
        return book is null ? null : Map(book);
    }

    public async Task<BookDto> CreateAsync(CreateBookRequest request, CancellationToken cancellationToken = default)
    {
        ValidateRequest(request.Title, request.AuthorId, request.Isbn, request.PublishedYear);

        var book = new Book
        {
            Id = Guid.NewGuid().ToString("N")[..8],
            Title = request.Title.Trim(),
            AuthorId = request.AuthorId.Trim(),
            Isbn = request.Isbn.Trim(),
            PublishedYear = request.PublishedYear
        };

        var created = await repository.AddAsync(book, cancellationToken);
        return Map(created);
    }

    public async Task<BookDto?> UpdateAsync(string id, UpdateBookRequest request, CancellationToken cancellationToken = default)
    {
        ValidateRequest(request.Title, request.AuthorId, request.Isbn, request.PublishedYear);

        var updated = await repository.UpdateAsync(
            id,
            new Book
            {
                Id = id,
                Title = request.Title.Trim(),
                AuthorId = request.AuthorId.Trim(),
                Isbn = request.Isbn.Trim(),
                PublishedYear = request.PublishedYear
            },
            cancellationToken);

        return updated is null ? null : Map(updated);
    }

    public Task<bool> DeleteAsync(string id, CancellationToken cancellationToken = default) =>
        repository.DeleteAsync(id, cancellationToken);

    private static void ValidateRequest(string title, string authorId, string isbn, int publishedYear)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            throw new ArgumentException("Title is required.", nameof(title));
        }

        if (string.IsNullOrWhiteSpace(authorId))
        {
            throw new ArgumentException("AuthorId is required.", nameof(authorId));
        }

        if (string.IsNullOrWhiteSpace(isbn))
        {
            throw new ArgumentException("Isbn is required.", nameof(isbn));
        }

        if (publishedYear < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(publishedYear), "PublishedYear must be non-negative.");
        }
    }

    private static BookDto Map(Book book) =>
        new(book.Id, book.Title, book.AuthorId, book.Isbn, book.PublishedYear);
}
