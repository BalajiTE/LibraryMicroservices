using Members.Application.DTOs;

namespace Members.Application.Services;

public interface IMemberService
{
    Task<IReadOnlyList<MemberDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<MemberDto?> GetByIdAsync(string id, CancellationToken cancellationToken = default);
    Task<MemberDto> CreateAsync(CreateMemberRequest request, CancellationToken cancellationToken = default);
    Task<MemberDto?> UpdateAsync(string id, UpdateMemberRequest request, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(string id, CancellationToken cancellationToken = default);
}
