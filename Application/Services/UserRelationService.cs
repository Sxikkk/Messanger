using Application.Interfaces;
using Domain.Entities;
using Domain.Interfaces;
using Domain.ValueObjects;

namespace Application.Services;

public class UserRelationService : IUserRelationService
{
    private readonly IUserRelationRepository _userRelationRepository;
    private readonly IUserRepository _userRepository;
    
    public UserRelationService(IUserRelationRepository userRelationRepository, IUserRepository userRepository)
    {
        _userRelationRepository = userRelationRepository;
        _userRepository = userRepository;
    }

    public async Task SendFriendRequestAsync(string targetUsername, Guid currentUserId, CancellationToken ct)
    {
        var targetUserId = await _userRepository.GetUserIdByUsernameAsync(targetUsername, ct);
        
        await EnsureNotBlockedAsync(currentUserId, targetUserId, ct);

        if (await _userRelationRepository.ExistsAsync(currentUserId, targetUserId, ERelationType.Friend, ct))
            throw new InvalidOperationException("Friend request already exists");

        var relation = new UserRelation(
            currentUserId,
            targetUserId,
            ERelationType.Friend,
            ERelationStatus.Pending
        );

        await _userRelationRepository.AddAsync(relation, ct);
        await _userRelationRepository.SaveChangesAsync(ct);
    }
    public async Task AcceptFriendRequestAsync(string targetUsername, Guid currentUserId, CancellationToken ct)
    {
        var requestUserId =  await _userRepository.GetUserIdByUsernameAsync(targetUsername, ct);
        
        var incoming = await _userRelationRepository.GetAsync(
            requestUserId,
            currentUserId,
            ERelationType.Friend,
            ct
        );

        if (incoming is null || incoming.Status != ERelationStatus.Pending)
            throw new InvalidOperationException("No pending request");

        incoming.Accept();

        var reverse = new UserRelation(
            currentUserId,
            requestUserId,
            ERelationType.Friend,
            ERelationStatus.Accepted
        );

        await _userRelationRepository.AddAsync(reverse, ct);
        await _userRelationRepository.SaveChangesAsync(ct);
    }

    public async Task RejectFriendRequestAsync(string targetUsername, Guid currentUserId, CancellationToken ct)
    {
        var requestUserId =  await _userRepository.GetUserIdByUsernameAsync(targetUsername, ct);

        var incoming = await _userRelationRepository.GetAsync(
            requestUserId,
            currentUserId,
            ERelationType.Friend,
            ct
        );

        if (incoming is null || incoming.Status != ERelationStatus.Pending)
            throw new InvalidOperationException("No pending request");

        incoming.Reject();

        await _userRelationRepository.SaveChangesAsync(ct);
    }

    public async Task RemoveFriendAsync(string targetUsername, Guid currentUserId, CancellationToken ct)
    {
        var targetUserId =  await _userRepository.GetUserIdByUsernameAsync(targetUsername, ct);
       
        await _userRelationRepository.RemoveRangeAsync(currentUserId, targetUserId, ERelationType.Friend, ct);
        await _userRelationRepository.RemoveRangeAsync(targetUserId, currentUserId, ERelationType.Friend, ct);

        await _userRelationRepository.SaveChangesAsync(ct);
    }

    public async Task BlockUserAsync(string targetUsername, Guid currentUserId, CancellationToken ct)
    {
        var targetUserId =  await _userRepository.GetUserIdByUsernameAsync(targetUsername, ct);

        await RemoveFriendAsync(targetUsername, currentUserId, ct);

        var block = new UserRelation(
            currentUserId,
            targetUserId,
            ERelationType.Block
        );

        await _userRelationRepository.AddAsync(block, ct);
        await _userRelationRepository.SaveChangesAsync(ct);
    }

    public async Task UnblockUserAsync(string targetUsername, Guid currentUserId, CancellationToken ct)
    {
        var targetUserId =  await _userRepository.GetUserIdByUsernameAsync(targetUsername, ct);
        
        await _userRelationRepository.RemoveRangeAsync(
            currentUserId,
            targetUserId,
            ERelationType.Block,
            ct
        );

        await _userRelationRepository.SaveChangesAsync(ct);
    }

    public async Task<IReadOnlyList<string>> GetFriendsAsync(Guid currentUserId, CancellationToken ct)
    {
        var relations = await _userRelationRepository.GetOutgoingAsync(currentUserId, ERelationType.Friend, ct);

        var friendsIds = relations
            .Where(r => r.Status == ERelationStatus.Accepted)
            .Select(r => r.TargetUserId)
            .ToList();

        if (friendsIds.Count == 0) return Array.Empty<string>();

        var friendsDict = await _userRepository.GetUsernamesByIdsAsync(friendsIds, ct);
        return friendsIds.Where(friendsDict.ContainsKey)
            .Select(id => friendsDict[id])
            .ToList();
    }

    public async Task<IReadOnlyList<string>> GetIncomingFriendRequestsAsync(Guid currentUserId, CancellationToken ct)
    {
        var relations = await _userRelationRepository.GetIncomingAsync(
            currentUserId, 
            ERelationType.Friend, 
            ERelationStatus.Pending, 
            ct
        );

        var requesterIds = relations.Select(r => r.UserId).ToList();
        if (requesterIds.Count == 0)
            return Array.Empty<string>();

        var usernamesDict = await _userRepository.GetUsernamesByIdsAsync(requesterIds, ct);

        return requesterIds
            .Where(id => usernamesDict.ContainsKey(id))
            .Select(id => usernamesDict[id])
            .ToList();
    }


    private async Task EnsureNotBlockedAsync(
        Guid userId,
        Guid targetUserId,
        CancellationToken ct
    )
    {
        if (await _userRelationRepository.ExistsAsync(userId, targetUserId, ERelationType.Block, ct) ||
            await _userRelationRepository.ExistsAsync(targetUserId, userId, ERelationType.Block, ct))
        {
            throw new InvalidOperationException("User is blocked");
        }
    }
}