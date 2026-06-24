using Members.Domain.Entities;
using Members.Domain.Repositories;
using Shared.BuildingBlocks.Persistence;

namespace Members.Infrastructure.Persistence;

public sealed class JsonMemberRepository : JsonFileRepository<Member, string>, IMemberRepository
{
    public JsonMemberRepository(string filePath) : base(filePath)
    {
    }

    protected override string GetKey(Member entity) => entity.Id;

    protected override Member WithKey(Member entity, string key) =>
        new()
        {
            Id = key,
            Name = entity.Name,
            Email = entity.Email
        };

    public Task<IReadOnlyList<Member>> GetAllAsync(CancellationToken cancellationToken = default) =>
        base.GetAllAsync(cancellationToken);

    public Task<Member?> GetByIdAsync(string id, CancellationToken cancellationToken = default) =>
        base.GetByIdAsync(id, cancellationToken);

    public Task<Member> AddAsync(Member member, CancellationToken cancellationToken = default) =>
        base.AddAsync(member, cancellationToken);

    public Task<Member?> UpdateAsync(string id, Member member, CancellationToken cancellationToken = default) =>
        base.UpdateAsync(
            id,
            _ => new Member
            {
                Id = id,
                Name = member.Name,
                Email = member.Email
            },
            cancellationToken);

    public Task<bool> DeleteAsync(string id, CancellationToken cancellationToken = default) =>
        base.DeleteAsync(id, cancellationToken);
}
