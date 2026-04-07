using Eclipse.Application.Jobs;
using Eclipse.Common.Clock;
using Eclipse.Common.Notifications;

using Newtonsoft.Json;

using Quartz;

namespace Eclipse.Application.MoodRecords.Report.Weekly;

internal sealed class WeeklyMoodReportScheduler : INotificationScheduler<WeeklyMoodReportJob, SchedulerOptions>
{
    private readonly ITimeProvider _timeProvider;

    public WeeklyMoodReportScheduler(ITimeProvider timeProvider)
    {
        _timeProvider = timeProvider;
    }

    public async Task Schedule(IScheduler scheduler, SchedulerOptions options, CancellationToken cancellationToken = default)
    {
        var job = await scheduler.GetJob<WeeklyMoodReportJob>(cancellationToken);

        var time = _timeProvider.Now.NextDayOfWeek(DayOfWeek.Sunday, true)
            .WithTime(NotificationConsts.Evening730PM)
            .Add(-options.Gmt);

        if (time < _timeProvider.Now)
        {
            time = time.NextDayOfWeek(DayOfWeek.Sunday);
        }

        var trigger = TriggerBuilder.Create()
            .ForJob(job)
            .WithIdentity(new TriggerKey($"{nameof(WeeklyMoodReportJob)}-{options.UserId}", options.UserId.ToString()))
            .UsingJobData("data", JsonConvert.SerializeObject(new UserIdJobData(options.UserId)))
            .WithCalendarIntervalSchedule(scheduler => scheduler.WithIntervalInWeeks(NotificationConsts.OneUnit))
            .StartAt(time)
            .Build();

        await scheduler.ScheduleJob(job, trigger, cancellationToken);
    }

    public Task Unschedule(IScheduler scheduler, SchedulerOptions options, CancellationToken cancellationToken = default)
    {
        return scheduler.UnscheduleJob(
            new TriggerKey($"{nameof(WeeklyMoodReportJob)}-{options.UserId}", options.UserId.ToString()),
            cancellationToken
        );
    }
}
