using Loans.Application.DTOs;
using Loans.Application.Integrations;
using Loans.Application.Services;
using Loans.Infrastructure.Persistence;
using Shared.Testing;

namespace Loans.Application.Tests;

public sealed class LoanServiceTests : IDisposable
{
    private readonly string _loansFilePath;
    private readonly LoanService _sut;

    public LoanServiceTests()
    {
        _loansFilePath = TempJsonFile.CreateCopy("loans.json");
        var loanRepository = new JsonLoanRepository(_loansFilePath);
        var membersApiClient = new TestMembersApiClient(
        [
            new MemberReference("m1", "Test Member")
        ]);
        _sut = new LoanService(loanRepository, membersApiClient);
    }

    [Fact]
    public async Task CreateAsync_AddsActiveLoan()
    {
        var created = await _sut.CreateAsync(
            new CreateLoanRequest("b3", "m1", new DateOnly(2026, 6, 1)));

        Assert.NotNull(created);
        Assert.Null(created.ReturnDate);
        Assert.Equal("m1", created.MemberId);
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
        if (File.Exists(_loansFilePath))
        {
            File.Delete(_loansFilePath);
        }
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
