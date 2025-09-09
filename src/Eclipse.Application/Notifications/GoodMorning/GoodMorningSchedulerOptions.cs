using Eclipse.Application.Jobs;

namespace Eclipse.Application.Notifications.GoodMorning;

internal record GoodMorningSchedulerOptions(Guid UserId, TimeSpan Gmt) : ISchedulerOptions;
