using System.Security.Claims;
using Application.Contracts.Auth;
using Application.Interfaces;
using Domain.Entities;
using Domain.Exceptions;
using Domain.Interfaces;

namespace Application.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IJwtService _jwtService;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ITokenHasher _tokenHasher;
    private readonly IEmailService _emailService;

    public AuthService(
        IUserRepository userRepository,
        IJwtService jwtService,
        IPasswordHasher passwordHasher,
        ITokenHasher tokenHasher,
        IEmailService emailService
    )
    {
        _userRepository = userRepository;
        _jwtService = jwtService;
        _passwordHasher = passwordHasher;
        _tokenHasher = tokenHasher;
        _emailService = emailService;
    }

    public async Task<TokenResponse?> CreateUserAsync(RegisterRequest request, CancellationToken cancellationToken)
    {
        var hashedPassword = _passwordHasher.HashPassword(request.Password);
        var refreshToken = _jwtService.GenerateRefreshToken();
        var user = new User(request.Email, hashedPassword, request.Login, _tokenHasher.HashToken(refreshToken),
            request.Name,
            request.Username,
            request.Surname);

        var claims = new[]
        {
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Name, user.Name),
            new Claim("Login", user.Login),
            new Claim("Username", user.Username),
        };

        var accessToken = _jwtService.GenerateAccessToken(claims);

        var emailConfirmationToken = _jwtService.GenerateAccessToken([new Claim(ClaimTypes.Email, user.Email)]);
        user = user with
        {
            EmailConfirmationToken = _tokenHasher.HashToken(emailConfirmationToken),
            IsEmailConfirmed = false
        };

        await _userRepository.AddUserAsync(user, cancellationToken);


        var confirmationLink =
            $"http://localhost:5136/api/auth/confirm-email?token={emailConfirmationToken}&email={Uri.EscapeDataString(request.Email)}";
        var htmlBody = $@"<h2>Добро пожаловать!</h2>
                      <p>Чтобы завершить регистрацию, перейдите по ссылке:</p>
                      <a href='{confirmationLink}'>Подтвердить email</a>";

        await _emailService.SendEmailAsync(request.Email, "Подтверждение регистрации", htmlBody, cancellationToken);

        return request.LoginAfter
            ? new TokenResponse
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken
            }
            : null;
    }

    public async Task<TokenResponse?> LoginUser(LoginRequest request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetUserByEmailOrLoginAsync(request.LoginOrEmail, cancellationToken);
        if (user is null) throw new InvalidCredentialsException(null);
        if (!_passwordHasher.VerifyHashedPassword(user.HashedPassword, request.Password))
            throw new InvalidCredentialsException(null);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Name, user.Name),
            new Claim("Login", user.Login),
        };

        var accessToken = _jwtService.GenerateAccessToken(claims);
        var refreshToken = _jwtService.GenerateRefreshToken();
        user.RefreshToken = _tokenHasher.HashToken(refreshToken);

        await _userRepository.UpdateUserAsync(user, cancellationToken);
        return new TokenResponse
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken
        };
    }

    public async Task<TokenResponse?> RefreshTokenAsync(RefreshRequest request, string login, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetUserByLoginAsync(login, cancellationToken);

        if (user == null) return null;

        if (!_tokenHasher.VerifyToken(request.RefreshToken, user.RefreshToken)) return null;

        var newRefreshToken = _jwtService.GenerateRefreshToken();
        user.RefreshToken = _tokenHasher.HashToken(newRefreshToken);
        await _userRepository.UpdateUserAsync(user, cancellationToken);

        var accessToken = _jwtService.GenerateAccessToken(GetClaims(user));

        return new TokenResponse
        {
            AccessToken = accessToken,
            RefreshToken = newRefreshToken
        };
    }

    public async Task<bool> ConfirmEmail(string token, string email, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(token) || string.IsNullOrWhiteSpace(email))
            return false;

        var user = await _userRepository.GetUserByEmailAsync(email, cancellationToken);
        if (user == null)
            throw new NotFoundException();

        if (user.IsEmailConfirmed)
            return true;

        if (string.IsNullOrEmpty(user.EmailConfirmationToken) ||
            !_tokenHasher.VerifyToken(token, user.EmailConfirmationToken))
            throw new InvalidCredentialsException("Invalid token or expired token");

        user.IsEmailConfirmed = true;
        user.EmailConfirmationToken = null;

        await _userRepository.UpdateUserAsync(user, cancellationToken);
        return true;
    }

    public async Task ResendConfirmationEmailAsync(string email, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetUserByEmailAsync(email, cancellationToken);

        if (user == null || user.IsEmailConfirmed)
            return;

        var confirmationToken = _jwtService.GenerateAccessToken([new Claim(ClaimTypes.Email, user.Email)]);
        var hashedToken = _tokenHasher.HashToken(confirmationToken);

        if (hashedToken != null) user.EmailConfirmationToken = _tokenHasher.HashToken(hashedToken);

        var confirmationLink =
            $"http://localhost:5136/api/auth/confirm-email?token={confirmationToken}&email={Uri.EscapeDataString(email)}";

        var htmlBody = $@"<h2>Подтверждение регистрации</h2>
                      <p>Вы запросили повторную отправку письма подтверждения.</p>
                      <p>Перейдите по ссылке, чтобы подтвердить email:</p>
                      <a href='{confirmationLink}'>Подтвердить email</a>
                      <p>Если вы не регистрировались — проигнорируйте это письмо.</p>";

        await _emailService.SendEmailAsync(email, "Повторное подтверждение email", htmlBody, cancellationToken);
    }

    private static IEnumerable<Claim> GetClaims(User user)
    {
        return
        [
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Login),
            new Claim(ClaimTypes.Email, user.Email)
        ];
    }
}