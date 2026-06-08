using Loans.Application.DTOs;
using Loans.Application.Services;
using Loans.Infrastructure.Persistence;
using Shared.Testing;

namespace Loans.Application.Tests;

public sealed class LoanServiceTests : IDisposable
{
    private readonly string _dataFilePath;
    private readonly LoanService _sut;

    public LoanServiceTests()
    {
        _dataFilePath = TempJsonFile.CreateCopy("loans.json");
        var repository = new JsonLoanRepository(_dataFilePath);
        _sut = new LoanService(repository);
    }

    [Fact]
    public async Task CreateAsync_AddsActiveLoan()
    {
        var created = await _sut.CreateAsync(
            new CreateLoanRequest("b3", "Charlie Brown", new DateOnly(2026, 6, 1)));

        Assert.NotNull(created);
        Assert.Null(created.ReturnDate);
    }

    [Fact]
    public async Task ReturnAsync_SetsReturnDate()
    {
        var returned = await _sut.ReturnAsync("l1", new ReturnLoanRequest(new DateOnly(2026, 6, 5)));

        Assert.NotNull(returned);
        Assert.Equal(new DateOnly(2026, 6, 5), returned.ReturnDate);
    }

    [Fact]
    public async Task ReturnAsync_ThrowsWhenAlreadyReturned()
    {
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _sut.ReturnAsync("l2", new ReturnLoanRequest(new DateOnly(2026, 6, 6))));
    }

    public void Dispose()
    {
        if (File.Exists(_dataFilePath))
        {
            File.Delete(_dataFilePath);
        }
    }
}
