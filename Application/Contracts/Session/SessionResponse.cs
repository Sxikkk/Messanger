namespace Application.Contracts.Session;

public record SessionResponse
{
    public string DeviceId  { get; init; }
    public string? DisplayName  { get; init; }
    public DateTimeOffset CreatedAt  { get; init; }
    public DateTimeOffset LastUsedAt  { get; init; }
    public bool IsExpired  { get; init; }

    public SessionResponse(string deviceId, string? displayName, DateTimeOffset createdAt, DateTimeOffset lastUsedAt, bool isExpired)
    {
        DeviceId = deviceId;
        DisplayName = displayName;
        CreatedAt = createdAt;
        LastUsedAt = lastUsedAt;
        IsExpired = isExpired;
    }
}