using System.Net;
using System.Net.Http.Json;
using Loans.Application.DTOs;
using Loans.Application.Integrations;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Loans.Api.Tests;

public sealed class LoansApiTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public LoansApiTests(WebApplicationFactory<Program> factory)
    {
        var loansFilePath = Shared.Testing.TempJsonFile.CreateCopy("loans.json");

        _client = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureAppConfiguration((_, config) =>
            {
                config.AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["Persistence:Provider"] = "Json",
                    ["Loans:DataFilePath"] = loansFilePath
                });
            });

            builder.ConfigureServices(services =>
            {
                var descriptors = services.Where(d => d.ServiceType == typeof(IMembersApiClient)).ToList();
                foreach (var descriptor in descriptors)
                {
                    services.Remove(descriptor);
                }

                services.AddSingleton<IMembersApiClient>(new TestMembersApiClient(
                [
                    new MemberReference("m1", "Test Member")
                ]));
            });
        }).CreateClient();
    }

    [Fact]
    public async Task GetLoans_ReturnsSeedLoans()
    {
        var response = await _client.GetAsync("/api/loans");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var loans = await response.Content.ReadFromJsonAsync<List<LoanDto>>();
        Assert.NotNull(loans);
        Assert.True(loans.Count >= 2);
    }

    [Fact]
    public async Task CreateLoan_ReturnsCreated()
    {
        var response = await _client.PostAsJsonAsync(
            "/api/loans",
            new CreateLoanRequest("b3", "m1", new DateOnly(2026, 6, 2)));

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    private sealed class TestMembersApiClient(IEnumerable<MemberReference> members) : IMembersApiClient
    {
        private readonly Dictionary<string, string> _members = members.ToDictionary(member => member.Id, member => member.Name);

        public Task<MemberReference?> GetByIdAsync(string memberId, CancellationToken cancellationToken = default) =>
            Task.FromResult(_members.TryGetValue(memberId, out var name)
                ? new MemberReference(memberId, name)
                : null);

        public Task<IReadOnlyDictionary<string, string>> GetNameLookupAsync(CancellationToken cancellationToken = default) =>
            Task.FromResult<IReadOnlyDictionary<string, string>>(_members);
    }
}
