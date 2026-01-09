namespace Application.Contracts.User;

public record ShortUserInfoResponse
{
    public string Name { get; init; }
    public string Surname { get; init; }
    public string Username { get; init; }
    public string? AvatarUrl { get; init; }
    //TODO: перенести куда-то в extended или еще dto для "чужого" польозователя
    public string? Bio { get; init; }
};