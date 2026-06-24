using Auth.Application.DTOs;

namespace Auth.Application.Services;

public interface IRoleService
{
    Task<IReadOnlyList<RoleDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<RoleDto?> GetByIdAsync(string id, CancellationToken cancellationToken = default);
    Task<RoleDto> CreateAsync(CreateRoleRequest request, CancellationToken cancellationToken = default);
    Task<RoleDto?> UpdateAsync(string id, UpdateRoleRequest request, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(string id, CancellationToken cancellationToken = default);
}
