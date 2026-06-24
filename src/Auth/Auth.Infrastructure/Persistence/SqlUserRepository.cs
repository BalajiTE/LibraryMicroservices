using Auth.Domain.Entities;
using Auth.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using Shared.Persistence;
using Shared.Persistence.Entities;

namespace Auth.Infrastructure.Persistence;

public sealed class SqlUserRepository(LibraryDbContext context) : IUserRepository
{
    public async Task<IReadOnlyList<User>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var users = await context.Users.AsNoTracking().OrderBy(user => user.Username).ToListAsync(cancellationToken);
        return users.Select(Map).ToList();
    }

    public async Task<User?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        var user = await context.Users.AsNoTracking().FirstOrDefaultAsync(record => record.Id == id, cancellationToken);
        return user is null ? null : Map(user);
    }

    public async Task<User?> GetByUsernameAsync(string username, CancellationToken cancellationToken = default)
    {
        var user = await context.Users.AsNoTracking()
            .FirstOrDefaultAsync(record => record.Username == username, cancellationToken);
        return user is null ? null : Map(user);
    }

    public async Task<User> AddAsync(User user, CancellationToken cancellationToken = default)
    {
        context.Users.Add(Map(user));
        await context.SaveChangesAsync(cancellationToken);
        return user;
    }

    public async Task<User?> UpdateAsync(string id, User user, CancellationToken cancellationToken = default)
    {
        var existing = await context.Users.FirstOrDefaultAsync(record => record.Id == id, cancellationToken);
        if (existing is null)
        {
            return null;
        }

        existing.Username = user.Username;
        existing.Email = user.Email;
        existing.PasswordHash = user.PasswordHash;
        existing.IsActive = user.IsActive;
        await context.SaveChangesAsync(cancellationToken);
        return Map(existing);
    }

    public async Task<bool> DeleteAsync(string id, CancellationToken cancellationToken = default)
    {
        var existing = await context.Users.FirstOrDefaultAsync(record => record.Id == id, cancellationToken);
        if (existing is null)
        {
            return false;
        }

        context.Users.Remove(existing);
        await context.SaveChangesAsync(cancellationToken);
        return true;
    }

    private static User Map(UserRecord record) =>
        new()
        {
            Id = record.Id,
            Username = record.Username,
            Email = record.Email,
            PasswordHash = record.PasswordHash,
            IsActive = record.IsActive
        };

    private static UserRecord Map(User user) =>
        new()
        {
            Id = user.Id,
            Username = user.Username,
            Email = user.Email,
            PasswordHash = user.PasswordHash,
            IsActive = user.IsActive
        };
}
