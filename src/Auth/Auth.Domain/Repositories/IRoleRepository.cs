using Auth.Domain.Entities;

namespace Auth.Domain.Repositories;

public interface IRoleRepository
{
    Task<IReadOnlyList<Role>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Role?> GetByIdAsync(string id, CancellationToken cancellationToken = default);
    Task<Role?> GetByNameAsync(string name, CancellationToken cancellationToken = default);
    Task<Role> AddAsync(Role role, CancellationToken cancellationToken = default);
    Task<Role?> UpdateAsync(string id, Role role, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(string id, CancellationToken cancellationToken = default);
}
