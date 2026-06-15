using Authors.Application.DTOs;

namespace Authors.Application.Services;

public interface IAuthorService
{
    Task<IReadOnlyList<AuthorDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<AuthorDto?> GetByIdAsync(string id, CancellationToken cancellationToken = default);
    Task<AuthorDto> CreateAsync(CreateAuthorRequest request, CancellationToken cancellationToken = default);
    Task<AuthorDto?> UpdateAsync(string id, UpdateAuthorRequest request, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(string id, CancellationToken cancellationToken = default);
}
