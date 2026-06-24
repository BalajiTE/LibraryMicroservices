using Members.Application.DTOs;
using Members.Domain.Entities;
using Members.Domain.Repositories;

namespace Members.Application.Services;

public sealed class MemberService(IMemberRepository repository) : IMemberService
{
    public async Task<IReadOnlyList<MemberDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var members = await repository.GetAllAsync(cancellationToken);
        return members.Select(Map).ToList();
    }

    public async Task<MemberDto?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        var member = await repository.GetByIdAsync(id, cancellationToken);
        return member is null ? null : Map(member);
    }

    public async Task<MemberDto> CreateAsync(CreateMemberRequest request, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
        {
            throw new ArgumentException("Name is required.", nameof(request));
        }

        var member = new Member
        {
            Id = Guid.NewGuid().ToString("N")[..8],
            Name = request.Name.Trim(),
            Email = request.Email?.Trim()
        };

        var created = await repository.AddAsync(member, cancellationToken);
        return Map(created);
    }

    public async Task<MemberDto?> UpdateAsync(string id, UpdateMemberRequest request, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
        {
            throw new ArgumentException("Name is required.", nameof(request));
        }

        var updated = await repository.UpdateAsync(
            id,
            new Member
            {
                Id = id,
                Name = request.Name.Trim(),
                Email = request.Email?.Trim()
            },
            cancellationToken);

        return updated is null ? null : Map(updated);
    }

    public Task<bool> DeleteAsync(string id, CancellationToken cancellationToken = default) =>
        repository.DeleteAsync(id, cancellationToken);

    private static MemberDto Map(Member member) =>
        new(member.Id, member.Name, member.Email);
}
