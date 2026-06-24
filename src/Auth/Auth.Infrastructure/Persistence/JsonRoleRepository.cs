using Auth.Domain.Entities;
using Auth.Domain.Repositories;
using Shared.BuildingBlocks.Persistence;

namespace Auth.Infrastructure.Persistence;

public sealed class JsonRoleRepository : JsonFileRepository<Role, string>, IRoleRepository
{
    public JsonRoleRepository(string filePath) : base(filePath)
    {
    }

    protected override string GetKey(Role entity) => entity.Id;

    protected override Role WithKey(Role entity, string key) =>
        new()
        {
            Id = key,
            Name = entity.Name,
            Description = entity.Description
        };

    public new Task<IReadOnlyList<Role>> GetAllAsync(CancellationToken cancellationToken = default) =>
        base.GetAllAsync(cancellationToken);

    public new Task<Role?> GetByIdAsync(string id, CancellationToken cancellationToken = default) =>
        base.GetByIdAsync(id, cancellationToken);

    public async Task<Role?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        var roles = await base.GetAllAsync(cancellationToken);
        return roles.FirstOrDefault(role =>
            string.Equals(role.Name, name, StringComparison.OrdinalIgnoreCase));
    }

    public new Task<Role> AddAsync(Role role, CancellationToken cancellationToken = default) =>
        base.AddAsync(role, cancellationToken);

    public new Task<Role?> UpdateAsync(string id, Role role, CancellationToken cancellationToken = default) =>
        base.UpdateAsync(
            id,
            _ => new Role
            {
                Id = id,
                Name = role.Name,
                Description = role.Description
            },
            cancellationToken);

    public new Task<bool> DeleteAsync(string id, CancellationToken cancellationToken = default) =>
        base.DeleteAsync(id, cancellationToken);
}
