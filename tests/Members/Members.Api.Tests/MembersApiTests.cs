using System.Net;
using System.Net.Http.Json;
using Members.Application.DTOs;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;

namespace Members.Api.Tests;

public sealed class MembersApiTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public MembersApiTests(WebApplicationFactory<Program> factory)
    {
        var dataFilePath = Shared.Testing.TempJsonFile.CreateCopy("members.json");

        _client = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureAppConfiguration((_, config) =>
            {
                config.AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["Persistence:Provider"] = "Json",
                    ["Members:DataFilePath"] = dataFilePath
                });
            });
        }).CreateClient();
    }

    [Fact]
    public async Task GetMembers_ReturnsOkWithSeedData()
    {
        var response = await _client.GetAsync("/api/members");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var members = await response.Content.ReadFromJsonAsync<List<MemberDto>>();
        Assert.NotNull(members);
        Assert.True(members.Count >= 1);
    }

    [Fact]
    public async Task Health_ReturnsHealthy()
    {
        var response = await _client.GetAsync("/health");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}
