using Application.Contracts.User;
using Application.Interfaces;
using Domain.Interfaces;

namespace Application.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;

    public UserService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<ShortUserInfoResponse?> GetShortUserInfo(string username, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetUserByUsernameAsync(username, cancellationToken);
        ArgumentNullException.ThrowIfNull(user);
        return new ShortUserInfoResponse
        {
            Name = user.Name,
            Surname = user.Surname ?? string.Empty,
            Username = user.Username,
            AvatarUrl = user.Settings.ShowAvatar ? user.AvatarUrl : null,
            Bio = user.Settings.ShowBio ? user.Bio : null,
        };
    }

    public async Task<UserProfileResponse?> GetUserProfileInfo(Guid userId, CancellationToken cancellationToken)
    {
        var fullUserInfo = await _userRepository.GetUserByIdAsync(userId, cancellationToken);
        ArgumentNullException.ThrowIfNull(fullUserInfo);
        return new UserProfileResponse(fullUserInfo);
    }

    public async Task UpdateUserSettings(UpdateUserSettingsRequest request, Guid userId,
        CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetUserByIdAsync(userId, cancellationToken);
        ArgumentNullException.ThrowIfNull(user);

        if (request.ShowAvatar.HasValue) user.Settings.ShowAvatar = (bool)request.ShowAvatar;
        if (request.ShowBio.HasValue) user.Settings.ShowBio = (bool)request.ShowBio;

        await _userRepository.UpdateUserAsync(user, cancellationToken);
    }
}