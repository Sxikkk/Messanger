using Domain.Entities;
using Domain.Interfaces;
using Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace Persistance.Repositories;

public class UserRelationRepository : IUserRelationRepository
{
    private readonly MyAppDbContext _dbContext;

    public UserRelationRepository(MyAppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<UserRelation?> GetAsync(
        Guid userId,
        Guid targetUserId,
        RelationTypeEnum relationType,
        CancellationToken ct
    )
    {
        return _dbContext.UserRelations
            .SingleOrDefaultAsync(
                ur => ur.UserId == userId
                      && ur.TargetUserId == targetUserId
                      && ur.RelationType == relationType,
                ct
            );
    }

    public async Task<IReadOnlyList<UserRelation>> GetOutgoingAsync(
        Guid userId,
        RelationTypeEnum relationType,
        CancellationToken ct
    )
    {
        return await _dbContext.UserRelations
            .Where(ur => ur.UserId == userId && ur.RelationType == relationType)
            .ToListAsync(ct);
    }

    public async Task<IReadOnlyList<UserRelation>> GetIncomingAsync(Guid targetUserId, RelationTypeEnum relationType,
        RelationStatus? status, CancellationToken ct)
    {
        return await _dbContext.UserRelations
            .Where(ur => ur.TargetUserId == targetUserId && ur.RelationType == relationType && (status == null || ur.Status == status))
            .ToListAsync(ct);
    }

    public async Task<bool> ExistsAsync(Guid userId, Guid targetUserId, RelationTypeEnum relationType,
        CancellationToken ct)
    {
        return await _dbContext.UserRelations.AnyAsync(
            ur => ur.UserId == userId && ur.TargetUserId == targetUserId && ur.RelationType == relationType, ct);
    }

    public async Task AddAsync(UserRelation relation, CancellationToken ct)
    {
        await _dbContext.UserRelations.AddAsync(relation, ct);
        await _dbContext.SaveChangesAsync(ct);
    }

    public async Task RemoveAsync(UserRelation relation, CancellationToken ct)
    {
        _dbContext.UserRelations.Remove(relation);
        await _dbContext.SaveChangesAsync(ct);
    }

    public async Task SaveChangesAsync(CancellationToken ct)
    {
        await _dbContext.SaveChangesAsync(ct);
    }
}