using Authors.Domain.Entities;

namespace Authors.Domain.Repositories;

public interface IAuthorRepository
{
    Task<IReadOnlyList<Author>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Author?> GetByIdAsync(string id, CancellationToken cancellationToken = default);
    Task<Author> AddAsync(Author author, CancellationToken cancellationToken = default);
    Task<Author?> UpdateAsync(string id, Author author, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(string id, CancellationToken cancellationToken = default);
}
