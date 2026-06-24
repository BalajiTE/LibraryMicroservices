using Auth.Domain.Entities;

namespace Auth.Domain.Repositories;

public interface IUserRoleRepository
{
    Task<IReadOnlyList<UserRole>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<string>> GetRoleIdsForUserAsync(string userId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<string>> GetRoleNamesForUserAsync(string userId, CancellationToken cancellationToken = default);
    Task AssignAsync(string userId, string roleId, CancellationToken cancellationToken = default);
    Task<bool> RemoveAsync(string userId, string roleId, CancellationToken cancellationToken = default);
}
