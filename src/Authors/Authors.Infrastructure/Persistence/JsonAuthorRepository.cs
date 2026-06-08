using Authors.Domain.Entities;
using Authors.Domain.Repositories;
using Shared.BuildingBlocks.Persistence;

namespace Authors.Infrastructure.Persistence;

public sealed class JsonAuthorRepository : JsonFileRepository<Author, string>, IAuthorRepository
{
    public JsonAuthorRepository(string filePath) : base(filePath)
    {
    }

    protected override string GetKey(Author entity) => entity.Id;

    protected override Author WithKey(Author entity, string key) =>
        new()
        {
            Id = key,
            Name = entity.Name,
            Bio = entity.Bio
        };

    public Task<IReadOnlyList<Author>> GetAllAsync(CancellationToken cancellationToken = default) =>
        base.GetAllAsync(cancellationToken);

    public Task<Author?> GetByIdAsync(string id, CancellationToken cancellationToken = default) =>
        base.GetByIdAsync(id, cancellationToken);

    public Task<Author> AddAsync(Author author, CancellationToken cancellationToken = default) =>
        base.AddAsync(author, cancellationToken);

    public Task<Author?> UpdateAsync(string id, Author author, CancellationToken cancellationToken = default) =>
        base.UpdateAsync(
            id,
            _ => new Author
            {
                Id = id,
                Name = author.Name,
                Bio = author.Bio
            },
            cancellationToken);

    public Task<bool> DeleteAsync(string id, CancellationToken cancellationToken = default) =>
        base.DeleteAsync(id, cancellationToken);
}
