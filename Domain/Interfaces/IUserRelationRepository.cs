using Domain.Entities;
using Domain.ValueObjects;

namespace Domain.Interfaces;

public interface IUserRelationRepository
{
    Task<UserRelation?> GetAsync(
        Guid userId,
        Guid targetUserId,
        RelationTypeEnum relationType,
        CancellationToken ct
    );

    Task<IReadOnlyList<UserRelation>> GetOutgoingAsync(
        Guid userId,
        RelationTypeEnum relationType,
        CancellationToken ct
    );

    Task<IReadOnlyList<UserRelation>> GetIncomingAsync(
        Guid targetUserId,
        RelationTypeEnum relationType,
        RelationStatus? status,
        CancellationToken ct
    );

    Task<bool> ExistsAsync(
        Guid userId,
        Guid targetUserId,
        RelationTypeEnum relationType,
        CancellationToken ct
    );

    Task AddAsync(UserRelation relation, CancellationToken ct);

    Task RemoveAsync(UserRelation relation, CancellationToken ct);

    Task SaveChangesAsync(CancellationToken ct);
}