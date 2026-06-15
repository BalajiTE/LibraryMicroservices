using Authors.Application.DTOs;
using Authors.Domain.Entities;
using Authors.Domain.Repositories;

namespace Authors.Application.Services;

public sealed class AuthorService(IAuthorRepository repository) : IAuthorService
{
    public async Task<IReadOnlyList<AuthorDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var authors = await repository.GetAllAsync(cancellationToken);
        return authors.Select(Map).ToList();
    }

    public async Task<AuthorDto?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        var author = await repository.GetByIdAsync(id, cancellationToken);
        return author is null ? null : Map(author);
    }

    public async Task<AuthorDto> CreateAsync(CreateAuthorRequest request, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
        {
            throw new ArgumentException("Name is required.", nameof(request));
        }

        var author = new Author
        {
            Id = Guid.NewGuid().ToString("N")[..8],
            Name = request.Name.Trim(),
            Bio = request.Bio?.Trim()
        };

        var created = await repository.AddAsync(author, cancellationToken);
        return Map(created);
    }

    public async Task<AuthorDto?> UpdateAsync(string id, UpdateAuthorRequest request, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
        {
            throw new ArgumentException("Name is required.", nameof(request));
        }

        var updated = await repository.UpdateAsync(
            id,
            new Author
            {
                Id = id,
                Name = request.Name.Trim(),
                Bio = request.Bio?.Trim()
            },
            cancellationToken);

        return updated is null ? null : Map(updated);
    }

    public Task<bool> DeleteAsync(string id, CancellationToken cancellationToken = default) =>
        repository.DeleteAsync(id, cancellationToken);

    private static AuthorDto Map(Author author) =>
        new(author.Id, author.Name, author.Bio);
}
