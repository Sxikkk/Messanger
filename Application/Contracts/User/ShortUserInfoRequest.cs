namespace Application.Contracts.User;

public record ShortUserInfoRequest
{
    public string Username { get; init; }
}