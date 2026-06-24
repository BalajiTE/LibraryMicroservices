using System.Net.Http.Json;
using Loans.Application.Integrations;
using Loans.Infrastructure.Options;
using Microsoft.Extensions.Options;

namespace Loans.Infrastructure.Integrations;

public sealed class MembersApiClient(
    HttpClient httpClient,
    IOptions<MembersApiOptions> options) : IMembersApiClient
{
    private readonly string _baseUrl = options.Value.BaseUrl.TrimEnd('/');

    public async Task<MemberReference?> GetByIdAsync(string memberId, CancellationToken cancellationToken = default)
    {
        using var response = await httpClient.GetAsync(
            $"{_baseUrl}/api/members/{memberId}",
            cancellationToken);

        if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return null;
        }

        response.EnsureSuccessStatusCode();

        var member = await response.Content.ReadFromJsonAsync<MemberApiDto>(cancellationToken);
        return member is null ? null : new MemberReference(member.Id, member.Name);
    }

    public async Task<IReadOnlyDictionary<string, string>> GetNameLookupAsync(CancellationToken cancellationToken = default)
    {
        using var response = await httpClient.GetAsync($"{_baseUrl}/api/members", cancellationToken);
        response.EnsureSuccessStatusCode();

        var members = await response.Content.ReadFromJsonAsync<List<MemberApiDto>>(cancellationToken);
        return members?.ToDictionary(member => member.Id, member => member.Name)
            ?? new Dictionary<string, string>();
    }

    private sealed record MemberApiDto(string Id, string Name, string? Email);
}
