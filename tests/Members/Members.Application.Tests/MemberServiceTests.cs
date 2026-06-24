using Members.Application.DTOs;
using Members.Application.Services;
using Members.Infrastructure.Persistence;
using Shared.Testing;

namespace Members.Application.Tests;

public sealed class MemberServiceTests : IDisposable
{
    private readonly string _dataFilePath;
    private readonly MemberService _sut;

    public MemberServiceTests()
    {
        _dataFilePath = TempJsonFile.CreateCopy("members.json");
        var repository = new JsonMemberRepository(_dataFilePath);
        _sut = new MemberService(repository);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsSeedMembers()
    {
        var members = await _sut.GetAllAsync();

        Assert.NotEmpty(members);
        Assert.Contains(members, member => member.Id == "m1");
    }

    [Fact]
    public async Task CreateAsync_AddsMember()
    {
        var created = await _sut.CreateAsync(new CreateMemberRequest("Charlie Brown", "charlie@example.com"));

        Assert.False(string.IsNullOrWhiteSpace(created.Id));

        var members = await _sut.GetAllAsync();
        Assert.Contains(members, member => member.Id == created.Id && member.Name == "Charlie Brown");
    }

    public void Dispose()
    {
        if (File.Exists(_dataFilePath))
        {
            File.Delete(_dataFilePath);
        }
    }
}
