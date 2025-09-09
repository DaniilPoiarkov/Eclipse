using Eclipse.Application.Jobs;

namespace Eclipse.Application.MoodRecords.Report;

internal record MoodReportSchedulerOptions(Guid UserId, TimeSpan Gmt) : ISchedulerOptions;
