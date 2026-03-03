using Domain.ValueObjects;

namespace Domain.Entities;

public sealed record UserRelation : TimeTracking
{
    public Guid UserId { get; private set; }
    public Guid TargetUserId { get; private set; }

    public ERelationType ERelationType { get; set; }
    public ERelationStatus? Status { get; set; }

    public User User { get; private set; } = null!;
    public User TargetUser { get; private set; } = null!;

    public UserRelation(
        Guid userId,
        Guid targetUserId,
        ERelationType eRelationType,
        ERelationStatus? status = null
    )
    {
        if (userId == targetUserId)
            throw new ArgumentException("User cannot relate to himself");

        UserId = userId;
        TargetUserId = targetUserId;
        ERelationType = eRelationType;
        Status = status;
    }

    public void Accept()
    {
        if (ERelationType != ERelationType.Friend || Status != ERelationStatus.Pending)
            throw new InvalidOperationException("Relation cannot be accepted");

        Status = ERelationStatus.Accepted;
    }

    public void Reject()
    {
        if (ERelationType != ERelationType.Friend || Status != ERelationStatus.Pending)
            throw new InvalidOperationException("Relation cannot be rejected");

        Status = ERelationStatus.Rejected;
    }
}