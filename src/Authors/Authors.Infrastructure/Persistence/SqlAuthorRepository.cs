using Authors.Domain.Entities;
using Authors.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using Shared.Persistence;
using Shared.Persistence.Entities;

namespace Authors.Infrastructure.Persistence;

public sealed class SqlAuthorRepository(LibraryDbContext context) : IAuthorRepository
{
    public async Task<IReadOnlyList<Author>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var authors = await context.Authors
            .AsNoTracking()
            .OrderBy(author => author.Name)
            .ToListAsync(cancellationToken);

        return authors.Select(Map).ToList();
    }

    public async Task<Author?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        var author = await context.Authors
            .AsNoTracking()
            .FirstOrDefaultAsync(record => record.Id == id, cancellationToken);

        return author is null ? null : Map(author);
    }

    public async Task<Author> AddAsync(Author author, CancellationToken cancellationToken = default)
    {
        var record = Map(author);
        context.Authors.Add(record);
        await context.SaveChangesAsync(cancellationToken);
        return author;
    }

    public async Task<Author?> UpdateAsync(string id, Author author, CancellationToken cancellationToken = default)
    {
        var existing = await context.Authors.FirstOrDefaultAsync(record => record.Id == id, cancellationToken);
        if (existing is null)
        {
            return null;
        }

        existing.Name = author.Name;
        existing.Bio = author.Bio;
        await context.SaveChangesAsync(cancellationToken);
        return Map(existing);
    }

    public async Task<bool> DeleteAsync(string id, CancellationToken cancellationToken = default)
    {
        var existing = await context.Authors.FirstOrDefaultAsync(record => record.Id == id, cancellationToken);
        if (existing is null)
        {
            return false;
        }

        context.Authors.Remove(existing);
        await context.SaveChangesAsync(cancellationToken);
        return true;
    }

    private static Author Map(AuthorRecord record) =>
        new()
        {
            Id = record.Id,
            Name = record.Name,
            Bio = record.Bio
        };

    private static AuthorRecord Map(Author author) =>
        new()
        {
            Id = author.Id,
            Name = author.Name,
            Bio = author.Bio
        };
}
