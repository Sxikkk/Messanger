using Application.Contracts.Auth;

namespace Application.Interfaces;

public interface IAuthService
{
    Task<TokenResponse?> CreateUserAsync(RegisterRequest request, CancellationToken cancellationToken);
    Task<TokenResponse?> LoginUser(LoginRequest request, CancellationToken cancellationToken);
    Task<TokenResponse?> RefreshTokenAsync(RefreshRequest request, string login, CancellationToken cancellationToken);
    Task<bool> ConfirmEmail(string token, string email, CancellationToken cancellationToken);
    Task ResendConfirmationEmailAsync(string email, CancellationToken cancellationToken);
}