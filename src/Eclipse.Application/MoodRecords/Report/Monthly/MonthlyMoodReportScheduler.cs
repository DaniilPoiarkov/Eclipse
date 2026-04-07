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
        var job = await scheduler.GetOrAddDurableJob<MonthlyMoodReportJob>(cancellationToken);

        var time = _timeProvider.Now.LastDayOfTheMonth()
            .WithTime(NotificationConsts.Evening830PM)
            .Add(-options.Gmt);

        if (time < _timeProvider.Now)
        {
            time = time.AddMonths(NotificationConsts.OneUnit);
        }

        var trigger = TriggerBuilder.Create()
            .ForJob(job)
            .WithIdentity(new TriggerKey($"{nameof(MonthlyMoodReportJob)}-{options.UserId}", options.UserId.ToString()))
            .UsingJobData("data", JsonConvert.SerializeObject(new UserIdJobData(options.UserId)))
            .WithCalendarIntervalSchedule(scheduler => scheduler.WithIntervalInMonths(NotificationConsts.OneUnit))
            .StartAt(time)
            .Build();

        await scheduler.ScheduleJob(trigger, cancellationToken);
    }

    public Task Unschedule(IScheduler scheduler, SchedulerOptions options, CancellationToken cancellationToken = default)
    {
        return scheduler.UnscheduleJob(
            new TriggerKey($"{nameof(MonthlyMoodReportJob)}-{options.UserId}", options.UserId.ToString()),
            cancellationToken
        );
    }
}
