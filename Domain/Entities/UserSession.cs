using System.Text.Json.Serialization;
using Domain.ValueObjects;

namespace Domain.Entities;

public class UserSession
{
    public Guid Id { get; init; }
    public string DeviceId { get; init; }
    public Guid UserId { get; init; }

    public string? DisplayName { get; init; }

    public DateTimeOffset CreatedAt { get; init; }
    public DateTimeOffset LastUsedAt { get; set; }
    public RefreshToken RefreshToken { get; init; } = null!;
    [JsonIgnore] public User User { get; init; }

    private UserSession() { }
    
    public UserSession(
        string deviceId,
        Guid userId,
        RefreshToken refreshToken,
        string? displayName,
        DateTimeOffset createdAt,
        DateTimeOffset lastUsedAt)
    {
        Id = Guid.NewGuid();
        DeviceId = deviceId;
        UserId = userId;
        RefreshToken = refreshToken;
        DisplayName = displayName;
        CreatedAt = createdAt;
        LastUsedAt = lastUsedAt;
    }

    public void UpdateLastUsedAt()
    {
        LastUsedAt = DateTimeOffset.UtcNow;
    }
}