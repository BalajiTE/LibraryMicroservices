using System.Net;
using System.Net.Http.Json;
using Loans.Application.Integrations;
using Loans.Infrastructure.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Loans.Infrastructure.Integrations;

public sealed class MembersApiClient(
    HttpClient httpClient,
    IOptions<MembersApiOptions> options,
    ILogger<MembersApiClient> logger) : IMembersApiClient
{
    private readonly string _baseUrl = options.Value.BaseUrl.TrimEnd('/');

    public async Task<MemberReference?> GetByIdAsync(string memberId, CancellationToken cancellationToken = default)
    {
        try
        {
            using var response = await httpClient.GetAsync(
                $"{_baseUrl}/api/members/{memberId}",
                cancellationToken);

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return null;
            }

            if (!response.IsSuccessStatusCode)
            {
                logger.LogWarning(
                    "Members API returned {StatusCode} for member {MemberId}",
                    (int)response.StatusCode,
                    memberId);
                return null;
            }

            var member = await response.Content.ReadFromJsonAsync<MemberApiDto>(cancellationToken);
            return member is null ? null : new MemberReference(member.Id, member.Name);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Failed to resolve member {MemberId} from Members API", memberId);
            return null;
        }
    }

    public async Task<IReadOnlyDictionary<string, string>> GetNameLookupAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            using var response = await httpClient.GetAsync($"{_baseUrl}/api/members", cancellationToken);
            if (!response.IsSuccessStatusCode)
            {
                logger.LogWarning(
                    "Members API returned {StatusCode} for member lookup",
                    (int)response.StatusCode);
                return new Dictionary<string, string>();
            }

            var members = await response.Content.ReadFromJsonAsync<List<MemberApiDto>>(cancellationToken);
            return members?.ToDictionary(member => member.Id, member => member.Name)
                ?? new Dictionary<string, string>();
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Failed to load members from Members API");
            return new Dictionary<string, string>();
        }
    }

    private sealed record MemberApiDto(string Id, string Name, string? Email);
}
