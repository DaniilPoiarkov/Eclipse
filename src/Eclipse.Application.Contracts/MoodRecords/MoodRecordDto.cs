using Eclipse.Application.Contracts.Entities;

namespace Eclipse.Application.Contracts.MoodRecords;

public sealed class MoodRecordDto : EntityDto
{
    public Guid UserId { get; set; }

    public bool IsGood { get; set; }

    public DateTime CreatedAt { get; set; }
}
