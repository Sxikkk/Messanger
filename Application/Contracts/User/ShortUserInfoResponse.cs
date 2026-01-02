namespace Application.Contracts.User;

public record ShortUserInfoResponse
{
    public string Name { get; init; }
    public string Surname { get; init; }
    public string Username { get; init; }
    public string? AvatarUrl { get; init; }
    public string? Bio { get; init; }
};