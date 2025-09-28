using Eclipse.Application.Jobs;
using Eclipse.Common.Clock;
using Eclipse.Common.Notifications;

using Newtonsoft.Json;

using Quartz;

namespace Eclipse.Application.MoodRecords.Report;

internal sealed class MoodReportScheduler : INotificationScheduler<MoodReportJob, SchedulerOptions>
{
    private readonly ITimeProvider _timeProvider;

    public MoodReportScheduler(ITimeProvider timeProvider)
    {
        _timeProvider = timeProvider;
    }

    public async Task Schedule(IScheduler scheduler, SchedulerOptions options, CancellationToken cancellationToken = default)
    {
        var key = JobKey.Create($"{nameof(MoodReportJob)}-{options.UserId}");

        var job = JobBuilder.Create<MoodReportJob>()
            .WithIdentity(key)
            .UsingJobData("data", JsonConvert.SerializeObject(new MoodReportJobData(options.UserId)))
            .Build();

        var time = _timeProvider.Now.NextDayOfWeek(DayOfWeek.Sunday, true)
            .WithTime(NotificationConsts.Evening730PM)
            .Add(-options.Gmt);

        if (time < _timeProvider.Now)
        {
            time = time.NextDayOfWeek(DayOfWeek.Sunday);
        }

        var trigger = TriggerBuilder.Create()
            .ForJob(job)
            .WithCalendarIntervalSchedule(scheduler => scheduler.WithIntervalInWeeks(NotificationConsts.OneUnit))
            .StartAt(time)
            .Build();

        await scheduler.ScheduleJob(job, trigger, cancellationToken);
    }

    public Task Unschedule(IScheduler scheduler, SchedulerOptions options, CancellationToken cancellationToken = default)
    {
        var key = JobKey.Create($"{nameof(MoodReportJob)}-{options.UserId}");
        return scheduler.DeleteJob(key, cancellationToken);
    }
}
