namespace Application.Contracts.User;

public record UpdateUserSettingsRequest
{
    public bool? ShowBio { get; init; }
    public bool? ShowAvatar { get; init; }
}