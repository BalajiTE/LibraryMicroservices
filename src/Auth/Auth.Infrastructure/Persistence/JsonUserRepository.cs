using Auth.Domain.Entities;
using Auth.Domain.Repositories;
using Shared.BuildingBlocks.Persistence;

namespace Auth.Infrastructure.Persistence;

public sealed class JsonUserRepository : JsonFileRepository<User, string>, IUserRepository
{
    public JsonUserRepository(string filePath) : base(filePath)
    {
    }

    protected override string GetKey(User entity) => entity.Id;

    protected override User WithKey(User entity, string key) =>
        new()
        {
            Id = key,
            Username = entity.Username,
            Email = entity.Email,
            PasswordHash = entity.PasswordHash,
            IsActive = entity.IsActive
        };

    public new Task<IReadOnlyList<User>> GetAllAsync(CancellationToken cancellationToken = default) =>
        base.GetAllAsync(cancellationToken);

    public new Task<User?> GetByIdAsync(string id, CancellationToken cancellationToken = default) =>
        base.GetByIdAsync(id, cancellationToken);

    public async Task<User?> GetByUsernameAsync(string username, CancellationToken cancellationToken = default)
    {
        var users = await base.GetAllAsync(cancellationToken);
        return users.FirstOrDefault(user =>
            string.Equals(user.Username, username, StringComparison.OrdinalIgnoreCase));
    }

    public new Task<User> AddAsync(User user, CancellationToken cancellationToken = default) =>
        base.AddAsync(user, cancellationToken);

    public new Task<User?> UpdateAsync(string id, User user, CancellationToken cancellationToken = default) =>
        base.UpdateAsync(
            id,
            _ => new User
            {
                Id = id,
                Username = user.Username,
                Email = user.Email,
                PasswordHash = user.PasswordHash,
                IsActive = user.IsActive
            },
            cancellationToken);

    public new Task<bool> DeleteAsync(string id, CancellationToken cancellationToken = default) =>
        base.DeleteAsync(id, cancellationToken);
}
