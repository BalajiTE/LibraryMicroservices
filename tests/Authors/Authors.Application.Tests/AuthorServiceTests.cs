using Authors.Application.DTOs;
using Authors.Application.Services;
using Authors.Infrastructure.Persistence;
using Shared.Testing;

namespace Authors.Application.Tests;

public sealed class AuthorServiceTests : IDisposable
{
    private readonly string _dataFilePath;
    private readonly AuthorService _sut;

    public AuthorServiceTests()
    {
        _dataFilePath = TempJsonFile.CreateCopy("authors.json");
        var repository = new JsonAuthorRepository(_dataFilePath);
        _sut = new AuthorService(repository);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsSeedAuthors()
    {
        var authors = await _sut.GetAllAsync();

        Assert.Equal(3, authors.Count);
        Assert.Contains(authors, author => author.Name == "Jane Austen");
    }

    [Fact]
    public async Task CreateAsync_AddsAuthorToJsonFile()
    {
        var created = await _sut.CreateAsync(new CreateAuthorRequest("New Author", "Bio"));

        Assert.False(string.IsNullOrWhiteSpace(created.Id));

        var authors = await _sut.GetAllAsync();
        Assert.Equal(4, authors.Count);
        Assert.Contains(authors, author => author.Id == created.Id && author.Name == "New Author");
    }

    [Fact]
    public async Task CreateAsync_ThrowsWhenNameMissing()
    {
        await Assert.ThrowsAsync<ArgumentException>(() =>
            _sut.CreateAsync(new CreateAuthorRequest("  ", null)));
    }

    public void Dispose()
    {
        if (File.Exists(_dataFilePath))
        {
            File.Delete(_dataFilePath);
        }
    }
}
