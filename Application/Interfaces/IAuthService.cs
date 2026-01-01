using Application.Contracts.Auth;

namespace Application.Interfaces;

public interface IAuthService
{
    Task<TokenResponse?> CreateUserAsync(RegisterRequest request);
    Task<TokenResponse?> LoginUser(LoginRequest request);
    Task<TokenResponse?> RefreshTokenAsync(RefreshRequest request);
    Task<bool> ConfirmEmail(string token, string email);
    Task ResendConfirmationEmailAsync(ResendConfirmationRequest request);
}