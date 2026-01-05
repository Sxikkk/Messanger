namespace Domain.ValueObjects;

public record RefreshToken
{
    public string TokenHashed { get; init; }
    public DateTimeOffset ExpiresAt { get; init; }
};