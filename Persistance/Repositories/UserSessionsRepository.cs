using Domain.Entities;
using Domain.Exceptions;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Persistance.Repositories;

public class UserSessionRepository : IUserSessionRepository
{
    private readonly MyAppDbContext _dbContext;

    public UserSessionRepository(MyAppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<UserSession?> GetUserSessionByTokenAsync(string hashedToken, CancellationToken cancellationToken)
    {
        return await _dbContext.UserSessions.SingleOrDefaultAsync(us => us.RefreshToken.TokenHashed == hashedToken,
            cancellationToken: cancellationToken);
    }

    public async Task<UserSession?> GetUserSessionByDeviceIdAsync(string deviceId, CancellationToken cancellationToken)
    {
        return await _dbContext.UserSessions.FirstOrDefaultAsync(us => us.DeviceId == deviceId,
            cancellationToken: cancellationToken);
    }

    public async Task<bool> AddUserSessionAsync(UserSession userSession, CancellationToken cancellationToken)
    {
        try
        {
            await _dbContext.UserSessions.AddAsync(userSession, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);
            return true;
        }
        catch (DbUpdateException)
        {
            return false;
        }

    }

    public async Task RemoveUserSessionAsync(UserSession userSession, CancellationToken cancellationToken)
    {
        _dbContext.UserSessions.Remove(userSession);
    }

    public async Task RemoveRangeUserSessionAsync(UserSession userSession, CancellationToken cancellationToken)
    {
        _dbContext.UserSessions.RemoveRange(userSession);
    }

    public async Task<IReadOnlyList<UserSession>> GetUserSessionsByUserIdAsync(Guid userId,
        CancellationToken cancellationToken)
    {
        return await _dbContext.UserSessions.Where(us => us.UserId == userId)
            .ToListAsync(cancellationToken: cancellationToken);
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> IsExist(Guid userId, string deviceId)
    {
        return await _dbContext.UserSessions.AnyAsync(us => us.DeviceId == deviceId && us.UserId == userId);
    }
}