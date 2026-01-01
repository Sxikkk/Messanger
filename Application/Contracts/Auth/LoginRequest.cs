namespace Application.Contracts.Auth;

public sealed record LoginRequest
{
    public string LoginOrEmail { get; init; }
    public string Password { get; init; }
};