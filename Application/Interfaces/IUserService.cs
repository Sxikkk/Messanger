using Application.Contracts.User;

namespace Application.Interfaces;

public interface IUserService
{
    Task<ShortUserInfoResponse?> GetShortUserInfo(string username, CancellationToken cancellationToken);
    Task<UserProfileResponse?> GetUserProfileInfo(Guid userId, CancellationToken cancellationToken);
    Task UpdateUserSettings(UpdateUserSettingsRequest request, Guid userId, CancellationToken cancellationToken);
}