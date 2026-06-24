using Auth.Domain.Entities;
using Auth.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using Shared.Persistence;
using Shared.Persistence.Entities;

namespace Auth.Infrastructure.Persistence;

public sealed class SqlRoleRepository(LibraryDbContext context) : IRoleRepository
{
    public async Task<IReadOnlyList<Role>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var roles = await context.Roles.AsNoTracking().OrderBy(role => role.Name).ToListAsync(cancellationToken);
        return roles.Select(Map).ToList();
    }

    public async Task<Role?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        var role = await context.Roles.AsNoTracking().FirstOrDefaultAsync(record => record.Id == id, cancellationToken);
        return role is null ? null : Map(role);
    }

    public async Task<Role?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        var role = await context.Roles.AsNoTracking().FirstOrDefaultAsync(record => record.Name == name, cancellationToken);
        return role is null ? null : Map(role);
    }

    public async Task<Role> AddAsync(Role role, CancellationToken cancellationToken = default)
    {
        context.Roles.Add(Map(role));
        await context.SaveChangesAsync(cancellationToken);
        return role;
    }

    public async Task<Role?> UpdateAsync(string id, Role role, CancellationToken cancellationToken = default)
    {
        var existing = await context.Roles.FirstOrDefaultAsync(record => record.Id == id, cancellationToken);
        if (existing is null)
        {
            return null;
        }

        existing.Name = role.Name;
        existing.Description = role.Description;
        await context.SaveChangesAsync(cancellationToken);
        return Map(existing);
    }

    public async Task<bool> DeleteAsync(string id, CancellationToken cancellationToken = default)
    {
        var existing = await context.Roles.FirstOrDefaultAsync(record => record.Id == id, cancellationToken);
        if (existing is null)
        {
            return false;
        }

        context.Roles.Remove(existing);
        await context.SaveChangesAsync(cancellationToken);
        return true;
    }

    private static Role Map(RoleRecord record) =>
        new()
        {
            Id = record.Id,
            Name = record.Name,
            Description = record.Description
        };

    private static RoleRecord Map(Role role) =>
        new()
        {
            Id = role.Id,
            Name = role.Name,
            Description = role.Description
        };
}
