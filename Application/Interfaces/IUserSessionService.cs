using Application.Contracts.Session;
using DeviceDetectorNET;
using Domain.Entities;
using Domain.ValueObjects;

namespace Application.Interfaces;

public interface IUserSessionService
{
    UserSession CreateUserSession(User user, string deviceId, string userAgent, RefreshToken refreshToken);    
    Task<IReadOnlyList<SessionResponse>> GetUserSessionsByIdAsync(Guid userId, CancellationToken ct);
    Task<UserSession?> GetUserSessionsByTokenAsync(string tokenHashed, CancellationToken ct);
    Task AddUserSessionAsync(UserSession userSession, CancellationToken ct);
    Task UpdateSessionTokenAsync(UserSession? userSession, string newHashedToken, DateTimeOffset newExpired, CancellationToken ct);
    Task RemoveSessionAsync(string? hashedToken, string deviceId,  CancellationToken ct);
    Task TerminateSessionsAsync(Guid userId, string currentToken, CancellationToken ct);
    Task TerminateSessionAsync(Guid userId, string? currentRefreshToken, string deviceId, CancellationToken ct);
}