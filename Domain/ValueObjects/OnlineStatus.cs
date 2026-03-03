namespace Domain.ValueObjects;

public class OnlineStatus
{
    public EOnlineStatus Status { get; set; }
    public DateTimeOffset LastSeen { get; set; }

    public OnlineStatus(EOnlineStatus status)
    {
        Status = status;
        LastSeen = DateTimeOffset.UtcNow;
    }

    public void SetOnline()
    {
        Status = EOnlineStatus.Online;
        LastSeen = DateTimeOffset.UtcNow;
    }

    public void SetOffline()
    {
        Status = EOnlineStatus.Offline;
        LastSeen = DateTimeOffset.UtcNow;
    }

    public void SetUnknown()
    {
        Status = EOnlineStatus.Unknown;
        LastSeen = DateTimeOffset.UtcNow;
    }
}