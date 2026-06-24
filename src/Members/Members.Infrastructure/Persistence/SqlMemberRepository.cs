using Members.Domain.Entities;
using Members.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using Shared.Persistence;
using Shared.Persistence.Entities;

namespace Members.Infrastructure.Persistence;

public sealed class SqlMemberRepository(LibraryDbContext context) : IMemberRepository
{
    public async Task<IReadOnlyList<Member>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var members = await context.Members
            .AsNoTracking()
            .OrderBy(member => member.Name)
            .ToListAsync(cancellationToken);

        return members.Select(Map).ToList();
    }

    public async Task<Member?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        var member = await context.Members
            .AsNoTracking()
            .FirstOrDefaultAsync(record => record.Id == id, cancellationToken);

        return member is null ? null : Map(member);
    }

    public async Task<Member> AddAsync(Member member, CancellationToken cancellationToken = default)
    {
        context.Members.Add(Map(member));
        await context.SaveChangesAsync(cancellationToken);
        return member;
    }

    public async Task<Member?> UpdateAsync(string id, Member member, CancellationToken cancellationToken = default)
    {
        var existing = await context.Members.FirstOrDefaultAsync(record => record.Id == id, cancellationToken);
        if (existing is null)
        {
            return null;
        }

        existing.Name = member.Name;
        existing.Email = member.Email;
        await context.SaveChangesAsync(cancellationToken);
        return Map(existing);
    }

    public async Task<bool> DeleteAsync(string id, CancellationToken cancellationToken = default)
    {
        var existing = await context.Members.FirstOrDefaultAsync(record => record.Id == id, cancellationToken);
        if (existing is null)
        {
            return false;
        }

        context.Members.Remove(existing);
        await context.SaveChangesAsync(cancellationToken);
        return true;
    }

    private static Member Map(MemberRecord record) =>
        new()
        {
            Id = record.Id,
            Name = record.Name,
            Email = record.Email
        };

    private static MemberRecord Map(Member member) =>
        new()
        {
            Id = member.Id,
            Name = member.Name,
            Email = member.Email
        };
}
