namespace Application.Contracts.Auth;

public sealed record TokenResponse
{
    public string AccessToken { get; init; }
    public string RefreshToken { get; init; }
};