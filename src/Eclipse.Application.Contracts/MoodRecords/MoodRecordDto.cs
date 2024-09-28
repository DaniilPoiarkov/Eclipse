using Eclipse.Application.Contracts.Entities;
using Eclipse.Domain.Shared.MoodRecords;

namespace Eclipse.Application.Contracts.MoodRecords;

public sealed class MoodRecordDto : EntityDto
{
    public Guid UserId { get; set; }

    public MoodState State { get; set; }

    public DateTime CreatedAt { get; set; }
}
