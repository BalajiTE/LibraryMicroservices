using Auth.Application.Services;
using Microsoft.AspNetCore.Identity;

namespace Auth.Infrastructure.Security;

public sealed class PasswordHasherService : IPasswordHasher
{
    private readonly PasswordHasher<object> _hasher = new();

    public string HashPassword(string password) =>
        _hasher.HashPassword(null!, password);

    public bool VerifyPassword(string password, string passwordHash) =>
        _hasher.VerifyHashedPassword(null!, passwordHash, password) != PasswordVerificationResult.Failed;
}
