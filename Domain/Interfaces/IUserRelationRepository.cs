using Domain.Entities;
using Domain.ValueObjects;

namespace Domain.Interfaces;

public interface IUserRelationRepository
{
    Task<UserRelation?> GetAsync(
        Guid userId,
        Guid targetUserId,
        ERelationType eRelationType,
        CancellationToken ct
    );

    Task<IReadOnlyList<UserRelation>> GetOutgoingAsync(
        Guid userId,
        ERelationType eRelationType,
        CancellationToken ct
    );

    Task<IReadOnlyList<UserRelation>> GetIncomingAsync(
        Guid targetUserId,
        ERelationType eRelationType,
        ERelationStatus? status,
        CancellationToken ct
    );

    Task<bool> ExistsAsync(
        Guid userId,
        Guid targetUserId,
        ERelationType eRelationType,
        CancellationToken ct
    );

    Task AddAsync(UserRelation relation, CancellationToken ct);

    void RemoveAsync(UserRelation relation, CancellationToken ct);
    Task RemoveRangeAsync(
        Guid userId,
        Guid targetUserId,
        ERelationType eRelationType,
        CancellationToken ct
    );

    Task SaveChangesAsync(CancellationToken ct);
}