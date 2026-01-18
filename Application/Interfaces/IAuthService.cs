using Application.Contracts.Auth;

namespace Application.Interfaces;

public interface IAuthService
{
    Task<AuthResponse?> CreateUserAsync(RegisterRequest request, string userAgent, string deviceId, CancellationToken cancellationToken);
    Task<AuthResponse?> LoginUser(LoginRequest request, string userAgent, string deviceId, CancellationToken cancellationToken);
    Task<AuthResponse?> RefreshTokenAsync(string token, CancellationToken cancellationToken);
    Task<bool> ConfirmEmail(string token, string email, CancellationToken cancellationToken);
    Task ResendConfirmationEmailAsync(string email, CancellationToken cancellationToken);
    Task LogoutUserAsync(string token, string deviceId, CancellationToken cancellationToken);
}