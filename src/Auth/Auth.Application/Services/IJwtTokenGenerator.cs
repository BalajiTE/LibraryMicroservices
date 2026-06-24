namespace Auth.Application.Services;

public interface IJwtTokenGenerator
{
    string GenerateToken(string userId, string username, IEnumerable<string> roles);
}
