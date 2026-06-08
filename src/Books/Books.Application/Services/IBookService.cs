using Books.Application.DTOs;

namespace Books.Application.Services;

public interface IBookService
{
    Task<IReadOnlyList<BookDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<BookDto?> GetByIdAsync(string id, CancellationToken cancellationToken = default);
    Task<BookDto> CreateAsync(CreateBookRequest request, CancellationToken cancellationToken = default);
    Task<BookDto?> UpdateAsync(string id, UpdateBookRequest request, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(string id, CancellationToken cancellationToken = default);
}
