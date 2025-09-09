using Eclipse.Application.Jobs;

namespace Eclipse.Application.MoodRecords.Collection;

internal sealed record CollectMoodRecordSchedulerOptions(Guid UserId, TimeSpan Gmt) : ISchedulerOptions;
