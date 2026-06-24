using Auth.Domain.Entities;
using Auth.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using Shared.Persistence;
using Shared.Persistence.Entities;

namespace Auth.Infrastructure.Persistence;

public sealed class SqlUserRoleRepository(LibraryDbContext context) : IUserRoleRepository
{
    public async Task<IReadOnlyList<UserRole>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var assignments = await context.UserRoles.AsNoTracking().ToListAsync(cancellationToken);
        return assignments.Select(Map).ToList();
    }

    public async Task<IReadOnlyList<string>> GetRoleIdsForUserAsync(string userId, CancellationToken cancellationToken = default)
    {
        return await context.UserRoles.AsNoTracking()
            .Where(assignment => assignment.UserId == userId)
            .Select(assignment => assignment.RoleId)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<string>> GetRoleNamesForUserAsync(string userId, CancellationToken cancellationToken = default)
    {
        return await context.UserRoles.AsNoTracking()
            .Where(assignment => assignment.UserId == userId)
            .Join(
                context.Roles.AsNoTracking(),
                assignment => assignment.RoleId,
                role => role.Id,
                (_, role) => role.Name)
            .OrderBy(name => name)
            .ToListAsync(cancellationToken);
    }

    public async Task AssignAsync(string userId, string roleId, CancellationToken cancellationToken = default)
    {
        var exists = await context.UserRoles.AnyAsync(
            assignment => assignment.UserId == userId && assignment.RoleId == roleId,
            cancellationToken);

        if (exists)
        {
            return;
        }

        context.UserRoles.Add(new UserRoleRecord { UserId = userId, RoleId = roleId });
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> RemoveAsync(string userId, string roleId, CancellationToken cancellationToken = default)
    {
        var existing = await context.UserRoles.FirstOrDefaultAsync(
            assignment => assignment.UserId == userId && assignment.RoleId == roleId,
            cancellationToken);

        if (existing is null)
        {
            return false;
        }

        context.UserRoles.Remove(existing);
        await context.SaveChangesAsync(cancellationToken);
        return true;
    }

    private static UserRole Map(UserRoleRecord record) =>
        new()
        {
            UserId = record.UserId,
            RoleId = record.RoleId
        };
}
