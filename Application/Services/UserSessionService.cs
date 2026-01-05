using Application.Interfaces;
using DeviceDetectorNET;
using Domain.Entities;
using Domain.ValueObjects;

namespace Application.Services;

public class UserSessionService: IUserSessionService
{
    
    private string? GetUserAgentConvertedInfo(string userAgent)
    {
        var detector = new DeviceDetector(userAgent);
        detector.DiscardBotInformation();
        detector.Parse();
        
        if (detector.IsParsed())
        {
            var device = detector.GetDeviceName();     // e.g. "iPhone" или "Desktop"
            var brand = detector.GetBrandName();       // "Apple", "Samsung" и т.д.
            var model = detector.GetModel();           // "iPhone 15 Pro"
            var os = detector.GetOs().Match;                 // { Family = "iOS", ShortName = "IOS", Version = "17.2" }
            var browser = detector.GetClient().Match;        // { Name = "Chrome", Version = "120" }

            return $"{brand} {model ?? device} | {os} | {browser}";
        }
        
        return string.Empty;
    }

    public UserSession CreateUserSession(User user, string deviceId, string userAgent, RefreshToken refreshToken)
    {
        var displayName = GetUserAgentConvertedInfo(userAgent);
        return new UserSession(deviceId, user.Id, refreshToken, displayName, DateTimeOffset.UtcNow, DateTimeOffset.UtcNow);
    }

    public async Task<UserSession> GetUserSessionByIdAsync(User user)
    {
        throw new NotImplementedException();
    }
}