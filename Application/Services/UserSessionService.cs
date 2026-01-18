using Application.Contracts.Session;
using Application.Interfaces;
using DeviceDetectorNET;
using Domain.Entities;
using Domain.Exceptions;
using Domain.Interfaces;
using Domain.ValueObjects;

namespace Application.Services;

public class UserSessionService : IUserSessionService
{
    private readonly IUserSessionRepository _userSessionRepository;
    private readonly ITokenHasher  _tokenHasher;
    private readonly ICacheService _cacheService;

    public UserSessionService(IUserSessionRepository userSessionRepository, ITokenHasher tokenHasher, ICacheService cacheService)
    {
        _userSessionRepository = userSessionRepository;
        _tokenHasher = tokenHasher;
        _cacheService = cacheService;
    }

    private string GetUserAgentConvertedInfo(string userAgent)
    {
        var detector = new DeviceDetector(userAgent);
        detector.DiscardBotInformation();
        detector.Parse();

        if (!detector.IsParsed()) return string.Empty;

        var device = detector.GetDeviceName();
        var brand = detector.GetBrandName();
        var model = detector.GetModel();
        var os = detector.GetOs().Match;
        var browser = detector.GetClient().Match;

        return $"{brand} {model ?? device} | {os} | {browser}";
    }

    public UserSession CreateUserSession(User user, string deviceId, string userAgent, RefreshToken refreshToken)
    {
        var displayName = GetUserAgentConvertedInfo(userAgent);
        return new UserSession(deviceId, user.Id, refreshToken, displayName, DateTimeOffset.UtcNow,
            DateTimeOffset.UtcNow);
    }

    public async Task<IReadOnlyList<SessionResponse>> GetUserSessionsByIdAsync(Guid userId, CancellationToken ct = default)
    {
        var sessions = await _userSessionRepository.GetUserSessionsByUserIdAsync(userId, ct);
        return sessions.Select(s => new SessionResponse(s.DeviceId, s.DisplayName, s.CreatedAt, s.LastUsedAt, s.RefreshToken.IsExpired)).ToList();
    }

    public async Task<UserSession?> GetUserSessionsByTokenAsync(string tokenHashed, CancellationToken ct)
    {
        return await _userSessionRepository.GetUserSessionByTokenAsync(tokenHashed, ct);
    }

    public async Task AddUserSessionAsync(UserSession userSession, CancellationToken ct)
    {
        var exist = await _userSessionRepository.GetUserSessionByDeviceIdAsync(userSession.DeviceId, ct);
        if (exist is not null)
        {
            await _userSessionRepository.RemoveUserSessionAsync(exist, ct);
        }

        await _userSessionRepository.AddUserSessionAsync(userSession, ct);
        await _userSessionRepository.SaveChangesAsync(ct);
    }

    public async Task UpdateSessionTokenAsync(UserSession? session, string newHashedToken, DateTimeOffset newExpired,
        CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(session, "session not found");
        if (session.RefreshToken.IsExpired) throw new UnauthorizedAccessException();
        session.RefreshToken.UpdateToken(newHashedToken, newExpired);
        session.UpdateLastUsedAt();
        await _userSessionRepository.SaveChangesAsync(ct);
    }

    public async Task<UserSession?> RemoveSessionAsync(string? hashedToken, string deviceId, CancellationToken ct)
    {
        var session = hashedToken is null 
            ? await _userSessionRepository.GetUserSessionByDeviceIdAsync(deviceId, ct) 
            :  await _userSessionRepository.GetUserSessionByTokenAsync(hashedToken, ct);
        
        if (session != null) await _userSessionRepository.RemoveUserSessionAsync(session, ct);
        await _userSessionRepository.SaveChangesAsync(ct);
        return session;
    }

    public async Task TerminateSessionsAsync(Guid userId, string currentToken, CancellationToken ct)
    {
        var sessions = await _userSessionRepository.GetUserSessionsByUserIdAsync(userId, ct);
        foreach (var session in sessions)
        {
            if (currentToken != null && 
                _tokenHasher.VerifyToken(currentToken, session.RefreshToken.TokenHashed))
                continue;

            await _userSessionRepository.RemoveUserSessionAsync(session, ct);
            await _cacheService.RemoveAsync($"session:{session.Id}");
        }
        await _userSessionRepository.SaveChangesAsync(ct);
    }

    public async Task TerminateSessionAsync(Guid userId, string? currentRefreshToken, string deviceId, CancellationToken ct)
    {
        
        var session = await _userSessionRepository.GetUserSessionByDeviceIdAsync(deviceId, ct);
        if (session == null || session.UserId != userId) throw new NotFoundException();
        
        if (currentRefreshToken != null && 
            _tokenHasher.VerifyToken(currentRefreshToken, session.RefreshToken.TokenHashed))
            throw new InvalidOperationException("Cannot terminate session");

        await _userSessionRepository.RemoveUserSessionAsync(session, ct);
        await _userSessionRepository.SaveChangesAsync(ct);
        await _cacheService.RemoveAsync($"session:{session.Id}");
    }
}