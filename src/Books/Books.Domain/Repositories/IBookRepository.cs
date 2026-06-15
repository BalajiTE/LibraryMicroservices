using Books.Domain.Entities;

namespace Books.Domain.Repositories;

public interface IBookRepository
{
    Task<IReadOnlyList<Book>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Book?> GetByIdAsync(string id, CancellationToken cancellationToken = default);
    Task<Book> AddAsync(Book book, CancellationToken cancellationToken = default);
    Task<Book?> UpdateAsync(string id, Book book, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(string id, CancellationToken cancellationToken = default);
}
