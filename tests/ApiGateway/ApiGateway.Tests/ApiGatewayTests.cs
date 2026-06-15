using System.Net;
using Microsoft.AspNetCore.Mvc.Testing;

namespace ApiGateway.Tests;

public sealed class ApiGatewayTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public ApiGatewayTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Health_ReturnsHealthy()
    {
        var response = await _client.GetAsync("/health");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var body = await response.Content.ReadAsStringAsync();
        Assert.Contains("ApiGateway", body);
        Assert.Contains("Healthy", body);
    }

    [Fact]
    public async Task ReverseProxyConfiguration_IsLoaded()
    {
        var response = await _client.GetAsync("/api/authors");

        Assert.NotEqual(HttpStatusCode.NotFound, response.StatusCode);
    }
}
