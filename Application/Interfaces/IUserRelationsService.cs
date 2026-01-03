namespace Application.Interfaces;

public interface IUserRelationService
{
    Task SendFriendRequestAsync(
        string targetUsername,
        Guid currentUserId,
        CancellationToken ct
    );

    Task AcceptFriendRequestAsync(
        string targetUsername,
        Guid currentUserId,
        CancellationToken ct
    );

    Task RejectFriendRequestAsync(
        string targetUsername,
        Guid currentUserId,
        CancellationToken ct
    );

    Task RemoveFriendAsync(
        string targetUsername,
        Guid currentUserId,
        CancellationToken ct
    );

    Task BlockUserAsync(
        string targetUsername,
        Guid currentUserId,
        CancellationToken ct
    );

    Task UnblockUserAsync(
        string targetUsername,
        Guid currentUserId,
        CancellationToken ct
    );

    Task<IReadOnlyList<string>> GetFriendsAsync(
        Guid currentUserId,
        CancellationToken ct
    );

    /// GET /friends/requests
    Task<IReadOnlyList<string>> GetIncomingFriendRequestsAsync(
        Guid currentUserId,
        CancellationToken ct
    );
}