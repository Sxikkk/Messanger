using Domain.ValueObjects;

namespace Domain.Entities;

public sealed record UserRelation : TimeTracking
{
    public Guid UserId { get; private set; }
    public Guid TargetUserId { get; private set; }

    public RelationTypeEnum RelationType { get; set; }
    public RelationStatus? Status { get; set; }

    public User User { get; private set; } = null!;
    public User TargetUser { get; private set; } = null!;

    public UserRelation(
        Guid userId,
        Guid targetUserId,
        RelationTypeEnum relationType,
        RelationStatus? status = null
    )
    {
        if (userId == targetUserId)
            throw new ArgumentException("User cannot relate to himself");

        UserId = userId;
        TargetUserId = targetUserId;
        RelationType = relationType;
        Status = status;
    }

    public void Accept()
    {
        if (RelationType != RelationTypeEnum.Friend || Status != RelationStatus.Pending)
            throw new InvalidOperationException("Relation cannot be accepted");

        Status = RelationStatus.Accepted;
    }

    public void Reject()
    {
        if (RelationType != RelationTypeEnum.Friend || Status != RelationStatus.Pending)
            throw new InvalidOperationException("Relation cannot be rejected");

        Status = RelationStatus.Rejected;
    }
}