using Domain.Entities;

namespace Application.Contracts.User;

public record UserProfileResponse : ShortUserInfoResponse
{
    public string? Email { get; init; }
    public string? Login { get; init; }
    public UserSettings? Settings { get; init; }
    public bool IsEmailConfirmed { get; init; }

    public UserProfileResponse(Domain.Entities.User userInfo)
    {
        Email = userInfo.Email;
        Login = userInfo.Login;
        Bio = userInfo.Bio;
        Settings = userInfo.Settings;
        Name = userInfo.Name;
        Surname = userInfo.Surname ?? string.Empty;
        Username = userInfo.Username;
        IsEmailConfirmed = userInfo.IsEmailConfirmed;
        AvatarUrl = userInfo.AvatarUrl;
        Bio = userInfo.Bio;
    }
}