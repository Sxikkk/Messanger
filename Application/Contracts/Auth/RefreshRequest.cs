namespace Application.Contracts.Auth;

public sealed record RefreshRequest
{
    public string RefreshToken { get; init; }
};