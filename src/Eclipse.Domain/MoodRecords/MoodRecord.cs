using Eclipse.Domain.Shared.Entities;
using Eclipse.Domain.Shared.MoodRecords;

namespace Eclipse.Domain.MoodRecords;

public sealed class MoodRecord : Entity
{
    internal MoodRecord(Guid id, Guid userId, MoodState state, DateTime createdAt) : base(id)
    {
        UserId = userId;
        State = state;
        CreatedAt = createdAt;
    }

    private MoodRecord() { }

    public Guid UserId { get; private set; }

    public MoodState State { get; private set; }

    public DateTime CreatedAt { get; private set; }
}
