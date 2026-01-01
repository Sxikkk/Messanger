using Domain.ValueObjects;

namespace Domain.Entities;

public sealed record User : TimeTracking
{
    public Guid Id { get; init; }
    public string Name { get; init; }
    public string Login { get; init; }
    public string? Surname { get; init; }
    public string Email { get; init; }
    public string HashedPassword { get; init; }
    public string? RefreshToken { get; set; }
    public bool IsEmailConfirmed { get; set; } = false;
    public string? EmailConfirmationToken { get; set; }
    public bool? IsBlocked { get; init; }
    
    public User(string email, string hashedPassword, string login, string? refreshToken, string name, string? surname)
    {
        Id = Guid.NewGuid();
        Email = email;
        HashedPassword = hashedPassword;
        Login = login;
        RefreshToken = refreshToken;
        Name = name;
        Surname = surname ?? null;
        IsEmailConfirmed = false;
        EmailConfirmationToken = null;
    }
};