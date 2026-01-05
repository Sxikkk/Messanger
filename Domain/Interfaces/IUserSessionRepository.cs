using Domain.Entities;

namespace Domain.Interfaces;

public interface IUserSessionRepository
{
    Task<UserSession?> GetUserSessionByTokenAsync(string hashedToken, CancellationToken cancellationToken);
    Task<UserSession?> GetUserSessionByDeviceIdAsync(string deviceId, CancellationToken cancellationToken);
    Task<bool> AddUserSessionAsync(UserSession userSession, CancellationToken cancellationToken);
    Task RemoveUserSessionAsync(UserSession userSession, CancellationToken cancellationToken);
    Task RemoveRangeUserSessionAsync(UserSession userSession, CancellationToken cancellationToken);
    Task<IReadOnlyList<UserSession>> GetUserSessionsByUserIdAsync(Guid userId, CancellationToken cancellationToken);
    
    Task SaveChangesAsync(CancellationToken cancellationToken);
}