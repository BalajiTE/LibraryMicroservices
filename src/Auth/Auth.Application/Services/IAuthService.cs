using Auth.Application.DTOs;

namespace Auth.Application.Services;

public interface IAuthService
{
    Task<TokenResponse?> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default);
    Task<TokenResponse> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default);
}
