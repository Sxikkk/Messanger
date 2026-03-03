using Application.Contracts.Session;
using Application.Contracts.User;
using Domain.Entities;

namespace Application.Interfaces;

public interface IUserService
{
    Task<ShortUserInfoResponse?> GetShortUserInfo(string username, CancellationToken cancellationToken);
    Task<UserProfileResponse?> GetUserProfileInfo(Guid userId, CancellationToken cancellationToken);
    Task<User?> GetUserByUsername(string username, CancellationToken cancellationToken);
    Task UpdateUserSettings(UpdateUserSettingsRequest request, Guid userId, CancellationToken cancellationToken);
    Task<UserPresenceResponse> GetUserOnlineStatusAsync(string username, CancellationToken ct);

}