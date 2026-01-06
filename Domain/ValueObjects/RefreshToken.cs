namespace Domain.ValueObjects;

public class RefreshToken
{
    public string? TokenHashed { get; set; }
    public DateTimeOffset ExpiresAt { get; set; }
    public bool IsExpired => ExpiresAt < DateTimeOffset.UtcNow;

    public void UpdateToken(string tokenHashed, DateTimeOffset expiresAt)
    {
        ExpiresAt = expiresAt;
        TokenHashed = tokenHashed;
    }
};