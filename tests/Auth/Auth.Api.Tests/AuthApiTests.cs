using System.Net;
using System.Net.Http.Json;
using Auth.Application.DTOs;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Shared.Persistence;

namespace Auth.Api.Tests;

public sealed class AuthApiTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public AuthApiTests(WebApplicationFactory<Program> factory)
    {
        var usersPath = Shared.Testing.TempJsonFile.CreateCopy("users.json");
        var rolesPath = Shared.Testing.TempJsonFile.CreateCopy("roles.json");
        var userRolesPath = Shared.Testing.TempJsonFile.CreateCopy("userRoles.json");

        _client = factory.WithWebHostBuilder(builder =>
        {
            builder.UseSetting(DependencyInjection.ProviderConfigKey, nameof(PersistenceProvider.Json));
            builder.ConfigureAppConfiguration((_, config) =>
            {
                config.AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["Auth:UsersDataFilePath"] = usersPath,
                    ["Auth:RolesDataFilePath"] = rolesPath,
                    ["Auth:UserRolesDataFilePath"] = userRolesPath
                });
            });
        }).CreateClient();
    }

    [Fact]
    public async Task Login_WithValidCredentials_ReturnsToken()
    {
        var response = await _client.PostAsJsonAsync(
            "/api/auth/login",
            new LoginRequest("librarian", "Password123!"));

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var token = await response.Content.ReadFromJsonAsync<TokenResponse>();
        Assert.NotNull(token);
        Assert.False(string.IsNullOrWhiteSpace(token!.AccessToken));
    }

    [Fact]
    public async Task Health_ReturnsHealthy()
    {
        var response = await _client.GetAsync("/health");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}
