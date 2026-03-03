namespace Infrastructure;

public record OnlineStatusSettings
{
    public int HeartbeatMinutes { get; init; }
};