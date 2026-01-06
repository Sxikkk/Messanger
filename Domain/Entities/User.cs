using Domain.ValueObjects;

namespace Domain.Entities;

public sealed record User : TimeTracking
{
    public Guid Id { get; init; }
    public string Name { get; init; }
    public string Login { get; init; }
    public UserSettings Settings { get; init; }
    public string? Surname { get; init; }
    public string Email { get; init; }
    public string HashedPassword { get; init; }
    public bool IsEmailConfirmed { get; set; }
    public string? EmailConfirmationToken { get; set; }
    public bool? IsBlocked { get; init; }
    public Guid PublicId { get; init; }
    public string Username { get; set; }
    public string? Bio { get; set; }
    public string? AvatarUrl { get; set; }
    
    public ICollection<UserSession> Sessions { get; init; } = new List<UserSession>();
    public User(string email, string hashedPassword, string login, string name, string? username, string? surname)
    {
        Id = Guid.NewGuid();
        PublicId = Guid.NewGuid();
        Email = email;
        AvatarUrl = null;
        Username = username ?? $"user_{Guid.NewGuid().ToString("N")[..8]}";
        HashedPassword = hashedPassword;
        Login = login;
        Name = name;
        Bio = null;;
        Surname = surname ?? null;
        IsEmailConfirmed = false;
        EmailConfirmationToken = null;
        Settings = new UserSettings(Id);
    }
};