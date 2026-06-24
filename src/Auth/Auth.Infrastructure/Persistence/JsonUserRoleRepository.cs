using Auth.Domain.Entities;
using Auth.Domain.Repositories;
using Shared.BuildingBlocks.Persistence;

namespace Auth.Infrastructure.Persistence;

public sealed class JsonUserRoleRepository : JsonFileRepository<UserRole, string>, IUserRoleRepository
{
    private readonly IRoleRepository _roleRepository;

    public JsonUserRoleRepository(string filePath, IRoleRepository roleRepository) : base(filePath)
    {
        _roleRepository = roleRepository;
    }

    protected override string GetKey(UserRole entity) => $"{entity.UserId}:{entity.RoleId}";

    protected override UserRole WithKey(UserRole entity, string key) => entity;

    public new async Task<IReadOnlyList<UserRole>> GetAllAsync(CancellationToken cancellationToken = default) =>
        await base.GetAllAsync(cancellationToken);

    public async Task<IReadOnlyList<string>> GetRoleIdsForUserAsync(string userId, CancellationToken cancellationToken = default)
    {
        var assignments = await base.GetAllAsync(cancellationToken);
        return assignments
            .Where(assignment => assignment.UserId == userId)
            .Select(assignment => assignment.RoleId)
            .ToList();
    }

    public async Task<IReadOnlyList<string>> GetRoleNamesForUserAsync(string userId, CancellationToken cancellationToken = default)
    {
        var roleIds = await GetRoleIdsForUserAsync(userId, cancellationToken);
        var roles = await _roleRepository.GetAllAsync(cancellationToken);
        return roles
            .Where(role => roleIds.Contains(role.Id))
            .Select(role => role.Name)
            .OrderBy(name => name)
            .ToList();
    }

    public async Task AssignAsync(string userId, string roleId, CancellationToken cancellationToken = default)
    {
        var assignments = await base.GetAllAsync(cancellationToken);
        if (assignments.Any(assignment => assignment.UserId == userId && assignment.RoleId == roleId))
        {
            return;
        }

        await base.AddAsync(new UserRole { UserId = userId, RoleId = roleId }, cancellationToken);
    }

    public Task<bool> RemoveAsync(string userId, string roleId, CancellationToken cancellationToken = default) =>
        base.DeleteAsync($"{userId}:{roleId}", cancellationToken);
}
