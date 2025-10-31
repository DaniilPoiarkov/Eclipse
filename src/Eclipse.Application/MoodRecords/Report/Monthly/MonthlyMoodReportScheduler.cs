using Eclipse.Application.Jobs;
using Eclipse.Common.Clock;
using Eclipse.Common.Notifications;

using Newtonsoft.Json;

using Quartz;

namespace Eclipse.Application.MoodRecords.Report.Monthly;

internal sealed class MonthlyMoodReportScheduler : INotificationScheduler<MonthlyMoodReportJob, SchedulerOptions>
{
    private readonly ITimeProvider _timeProvider;

    public MonthlyMoodReportScheduler(ITimeProvider timeProvider)
    {
        _timeProvider = timeProvider;
    }

    public async Task Schedule(IScheduler scheduler, SchedulerOptions options, CancellationToken cancellationToken = default)
    {
        var key = JobKey.Create($"{nameof(MonthlyMoodReportJob)}-{options.UserId}");

        var job = JobBuilder.Create<MonthlyMoodReportJob>()
            .WithIdentity(key)
            .UsingJobData("data", JsonConvert.SerializeObject(new UserIdJobData(options.UserId)))
            .Build();

        var time = _timeProvider.Now.LastDayOfTheMonth()
            .WithTime(NotificationConsts.Evening830PM)
            .Add(-options.Gmt);

        if (time < _timeProvider.Now)
        {
            time = time.AddMonths(NotificationConsts.OneUnit);
        }

        var trigger = TriggerBuilder.Create()
            .ForJob(job)
            .WithCalendarIntervalSchedule(scheduler => scheduler.WithIntervalInMonths(NotificationConsts.OneUnit))
            .StartAt(time)
            .Build();

        await scheduler.ScheduleJob(job, trigger, cancellationToken);
    }

    public Task Unschedule(IScheduler scheduler, SchedulerOptions options, CancellationToken cancellationToken = default)
    {
        var key = JobKey.Create($"{nameof(MonthlyMoodReportJob)}-{options.UserId}");
        return scheduler.DeleteJob(key, cancellationToken);
    }
}
