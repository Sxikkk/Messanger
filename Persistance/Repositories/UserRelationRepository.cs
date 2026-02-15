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
        ERelationType eRelationType,
        CancellationToken ct
    )
    {
        return _dbContext.UserRelations
            .SingleOrDefaultAsync(
                ur => ur.UserId == userId
                      && ur.TargetUserId == targetUserId
                      && ur.ERelationType == eRelationType,
                ct
            );
    }

    public async Task<IReadOnlyList<UserRelation>> GetOutgoingAsync(
        Guid userId,
        ERelationType eRelationType,
        CancellationToken ct
    )
    {
        return await _dbContext.UserRelations
            .Where(ur => ur.UserId == userId && ur.ERelationType == eRelationType)
            .ToListAsync(ct);
    }

    public async Task<IReadOnlyList<UserRelation>> GetIncomingAsync(Guid targetUserId, ERelationType eRelationType,
        ERelationStatus? status, CancellationToken ct)
    {
        return await _dbContext.UserRelations
            .Where(ur => ur.TargetUserId == targetUserId && ur.ERelationType == eRelationType &&
                         (status == null || ur.Status == status))
            .ToListAsync(ct);
    }

    public async Task<bool> ExistsAsync(Guid userId, Guid targetUserId, ERelationType eRelationType,
        CancellationToken ct)
    {
        return await _dbContext.UserRelations.AnyAsync(
            ur => ur.UserId == userId && ur.TargetUserId == targetUserId && ur.ERelationType == eRelationType, ct);
    }

    public async Task AddAsync(UserRelation relation, CancellationToken ct)
    {
        await _dbContext.UserRelations.AddAsync(relation, ct);
    }

    public void RemoveAsync(UserRelation relation, CancellationToken ct)
    {
        _dbContext.UserRelations.Remove(relation);
    }

    public async Task RemoveRangeAsync(
        Guid userId,
        Guid targetUserId,
        ERelationType eRelationType,
        CancellationToken ct
    )
    {
        await _dbContext.UserRelations
            .Where(ur =>
                ur.UserId == userId &&
                ur.TargetUserId == targetUserId &&
                ur.ERelationType == eRelationType
            )
            .ExecuteDeleteAsync(ct);
    }
    public async Task SaveChangesAsync(CancellationToken ct)
    {
        await _dbContext.SaveChangesAsync(ct);
    }
}