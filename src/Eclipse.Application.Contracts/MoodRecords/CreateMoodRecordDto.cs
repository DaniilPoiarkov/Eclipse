using Eclipse.Domain.Shared.MoodRecords;

namespace Eclipse.Application.Contracts.MoodRecords;

public sealed class CreateMoodRecordDto
{
    public MoodState State { get; set; }
}
