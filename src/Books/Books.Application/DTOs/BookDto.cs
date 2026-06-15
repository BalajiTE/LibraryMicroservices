namespace Books.Application.DTOs;

public sealed record BookDto(
    string Id,
    string Title,
    string AuthorId,
    string Isbn,
    int PublishedYear);

public sealed record CreateBookRequest(
    string Title,
    string AuthorId,
    string Isbn,
    int PublishedYear);

public sealed record UpdateBookRequest(
    string Title,
    string AuthorId,
    string Isbn,
    int PublishedYear);
