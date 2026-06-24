using Auth.Application.DTOs;
using Auth.Domain.Entities;
using Auth.Domain.Repositories;

namespace Auth.Application.Services;

public sealed class RoleService(IRoleRepository roleRepository) : IRoleService
{
    public async Task<IReadOnlyList<RoleDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var roles = await roleRepository.GetAllAsync(cancellationToken);
        return roles.Select(Map).ToList();
    }

    public async Task<RoleDto?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        var role = await roleRepository.GetByIdAsync(id, cancellationToken);
        return role is null ? null : Map(role);
    }

    public async Task<RoleDto> CreateAsync(CreateRoleRequest request, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
        {
            throw new ArgumentException("Name is required.");
        }

        var existing = await roleRepository.GetByNameAsync(request.Name.Trim(), cancellationToken);
        if (existing is not null)
        {
            throw new ArgumentException("Role name is already taken.");
        }

        var role = new Role
        {
            Id = Guid.NewGuid().ToString("N")[..8],
            Name = request.Name.Trim(),
            Description = request.Description?.Trim()
        };

        var created = await roleRepository.AddAsync(role, cancellationToken);
        return Map(created);
    }

    public async Task<RoleDto?> UpdateAsync(string id, UpdateRoleRequest request, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
        {
            throw new ArgumentException("Name is required.");
        }

        var updated = await roleRepository.UpdateAsync(
            id,
            new Role
            {
                Id = id,
                Name = request.Name.Trim(),
                Description = request.Description?.Trim()
            },
            cancellationToken);

        return updated is null ? null : Map(updated);
    }

    public Task<bool> DeleteAsync(string id, CancellationToken cancellationToken = default) =>
        roleRepository.DeleteAsync(id, cancellationToken);

    private static RoleDto Map(Role role) => new(role.Id, role.Name, role.Description);
}
