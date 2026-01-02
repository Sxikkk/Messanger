namespace Application.Interfaces;

public interface IUserRelationService
{
    /// POST /friends/request/{targetUserId}
    Task SendFriendRequestAsync(
        Guid targetUserId,
        Guid currentUserId,
        CancellationToken ct
    );

    /// POST /friends/accept/{requestUserId}
    Task AcceptFriendRequestAsync(
        Guid requestUserId,
        Guid currentUserId,
        CancellationToken ct
    );

    /// POST /friends/reject/{requestUserId}
    Task RejectFriendRequestAsync(
        Guid requestUserId,
        Guid currentUserId,
        CancellationToken ct
    );

    /// DELETE /friends/{targetUserId}
    Task RemoveFriendAsync(
        Guid targetUserId,
        Guid currentUserId,
        CancellationToken ct
    );

    /// POST /users/{targetUserId}/block
    Task BlockUserAsync(
        Guid targetUserId,
        Guid currentUserId,
        CancellationToken ct
    );

    /// DELETE /users/{targetUserId}/block
    Task UnblockUserAsync(
        Guid targetUserId,
        Guid currentUserId,
        CancellationToken ct
    );

    /// GET /friends
    Task<IReadOnlyList<Guid>> GetFriendsAsync(
        Guid currentUserId,
        CancellationToken ct
    );

    /// GET /friends/requests
    Task<IReadOnlyList<Guid>> GetIncomingFriendRequestsAsync(
        Guid currentUserId,
        CancellationToken ct
    );
}