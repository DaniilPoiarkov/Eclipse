using Eclipse.Domain.Shared.Entities;

namespace Eclipse.Domain.MoodRecords;

public sealed class MoodRecord : Entity
{
    public MoodRecord(Guid id, Guid userId, bool isGood, DateTime createdAt) : base(id)
    {
        UserId = userId;
        IsGood = isGood;
        CreatedAt = createdAt;
    }

    public Guid UserId { get; private set; }

    public bool IsGood { get; private set; }

    public DateTime CreatedAt { get; private set; }
}
