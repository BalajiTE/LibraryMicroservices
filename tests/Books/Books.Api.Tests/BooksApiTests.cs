using System.Net;
using System.Net.Http.Json;
using Books.Application.DTOs;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;

namespace Books.Api.Tests;

public sealed class BooksApiTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public BooksApiTests(WebApplicationFactory<Program> factory)
    {
        var dataFilePath = Shared.Testing.TempJsonFile.CreateCopy("books.json");

        _client = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureAppConfiguration((_, config) =>
            {
                config.AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["Persistence:Provider"] = "Json",
                    ["Books:DataFilePath"] = dataFilePath
                });
            });
        }).CreateClient();
    }

    [Fact]
    public async Task GetBookById_ReturnsBook()
    {
        var response = await _client.GetAsync("/api/books/b1");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var book = await response.Content.ReadFromJsonAsync<BookDto>();
        Assert.NotNull(book);
        Assert.Equal("Pride and Prejudice", book.Title);
    }

    [Fact]
    public async Task GetMissingBook_ReturnsNotFound()
    {
        var response = await _client.GetAsync("/api/books/missing");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
