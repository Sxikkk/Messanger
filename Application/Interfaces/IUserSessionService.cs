using DeviceDetectorNET;
using Domain.Entities;
using Domain.ValueObjects;

namespace Application.Interfaces;

public interface IUserSessionService
{
    UserSession CreateUserSession(User user, string deviceId, string userAgent, RefreshToken refreshToken);
    
    Task<UserSession> GetUserSessionByIdAsync(User user);
    Task AddUserSessionAsync(UserSession userSession, RefreshToken refreshToken);
}