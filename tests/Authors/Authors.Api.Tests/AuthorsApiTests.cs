using System.Net;
using System.Net.Http.Json;
using Authors.Application.DTOs;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;

namespace Authors.Api.Tests;

public sealed class AuthorsApiTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public AuthorsApiTests(WebApplicationFactory<Program> factory)
    {
        var dataFilePath = Shared.Testing.TempJsonFile.CreateCopy("authors.json");

        _client = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureAppConfiguration((_, config) =>
            {
                config.AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["Persistence:Provider"] = "Json",
                    ["Authors:DataFilePath"] = dataFilePath
                });
            });
        }).CreateClient();
    }

    [Fact]
    public async Task GetAuthors_ReturnsOkWithSeedData()
    {
        var response = await _client.GetAsync("/api/authors");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var authors = await response.Content.ReadFromJsonAsync<List<AuthorDto>>();
        Assert.NotNull(authors);
        Assert.True(authors.Count >= 3);
    }

    [Fact]
    public async Task Health_ReturnsHealthy()
    {
        var response = await _client.GetAsync("/health");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}
