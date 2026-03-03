namespace Application.Contracts.Session;

public record UserPresenceResponse
{
    public string Username { get; set; }
    public bool IsOnline { get; set; }
}