using Books.Application.DTOs;
using Books.Application.Services;
using Books.Infrastructure.Persistence;
using Shared.Testing;

namespace Books.Application.Tests;

public sealed class BookServiceTests : IDisposable
{
    private readonly string _dataFilePath;
    private readonly BookService _sut;

    public BookServiceTests()
    {
        _dataFilePath = TempJsonFile.CreateCopy("books.json");
        var repository = new JsonBookRepository(_dataFilePath);
        _sut = new BookService(repository);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsExistingBook()
    {
        var book = await _sut.GetByIdAsync("b1");

        Assert.NotNull(book);
        Assert.Equal("Pride and Prejudice", book.Title);
    }

    [Fact]
    public async Task UpdateAsync_UpdatesBookInJsonFile()
    {
        var updated = await _sut.UpdateAsync(
            "b2",
            new UpdateBookRequest("Nineteen Eighty-Four", "a2", "978-0451524935", 1949));

        Assert.NotNull(updated);
        Assert.Equal("Nineteen Eighty-Four", updated.Title);
    }

    [Fact]
    public async Task DeleteAsync_RemovesBook()
    {
        var deleted = await _sut.DeleteAsync("b3");

        Assert.True(deleted);
        Assert.Null(await _sut.GetByIdAsync("b3"));
    }

    public void Dispose()
    {
        if (File.Exists(_dataFilePath))
        {
            File.Delete(_dataFilePath);
        }
    }
}
