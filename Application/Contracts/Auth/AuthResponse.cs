namespace Application.Contracts.Auth;

public sealed record AuthResponse
{
    public string AccessToken { get; init; }
    public string RefreshToken { get; init; }
    public string SessionId { get; init; }
};