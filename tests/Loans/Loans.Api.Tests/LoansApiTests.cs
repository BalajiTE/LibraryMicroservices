using System.Net;
using System.Net.Http.Json;
using Loans.Application.DTOs;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;

namespace Loans.Api.Tests;

public sealed class LoansApiTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public LoansApiTests(WebApplicationFactory<Program> factory)
    {
        var dataFilePath = Shared.Testing.TempJsonFile.CreateCopy("loans.json");

        _client = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureAppConfiguration((_, config) =>
            {
                config.AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["Loans:DataFilePath"] = dataFilePath
                });
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
            new CreateLoanRequest("b3", "Test Borrower", new DateOnly(2026, 6, 2)));

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }
}
