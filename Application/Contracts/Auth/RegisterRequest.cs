using System.ComponentModel.DataAnnotations;

namespace Application.Contracts.Auth;

public sealed record RegisterRequest
{
    [EmailAddress]
    public string Email { get; init; }
    public string Password { get; init; }
    public string Name { get; init; }
    public string Login { get; init; }
    public string? Surname { get; init; }
    public string? Username { get; init; }
    public bool LoginAfter { get; init; } = true;
};