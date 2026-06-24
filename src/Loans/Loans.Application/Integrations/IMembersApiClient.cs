namespace Loans.Application.Integrations;

public sealed record MemberReference(string Id, string Name);

public interface IMembersApiClient
{
    Task<MemberReference?> GetByIdAsync(string memberId, CancellationToken cancellationToken = default);
    Task<IReadOnlyDictionary<string, string>> GetNameLookupAsync(CancellationToken cancellationToken = default);
}
