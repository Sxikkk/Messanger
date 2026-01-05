using Application.Contracts.Auth;

namespace Application.Interfaces;

public interface IAuthService
{
    Task<TokenResponse?> CreateUserAsync(RegisterRequest request, string userAgent, CancellationToken cancellationToken);
    Task<TokenResponse?> LoginUser(LoginRequest request, CancellationToken cancellationToken);
    Task<TokenResponse?> RefreshTokenAsync(string token, string login, CancellationToken cancellationToken);
    Task<bool> ConfirmEmail(string token, string email, CancellationToken cancellationToken);
    Task ResendConfirmationEmailAsync(string email, CancellationToken cancellationToken);
}