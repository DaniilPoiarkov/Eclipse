using Eclipse.Application.Reminders;
using Eclipse.Common.Clock;
using Eclipse.Common.Notifications;

using Newtonsoft.Json;

using Quartz;

namespace Eclipse.Application.MoodRecords.Report;

internal sealed class MoodReportScheduler : INotificationScheduler<MoodReportJob, MoodReportSchedulerOptions>
{
    private readonly ITimeProvider _timeProvider;

    public MoodReportScheduler(ITimeProvider timeProvider)
    {
        _timeProvider = timeProvider;
    }

    public async Task Schedule(IScheduler scheduler, MoodReportSchedulerOptions options, CancellationToken cancellationToken = default)
    {
        var key = JobKey.Create($"{nameof(MoodReportJob)}-{options.UserId}");

        var job = JobBuilder.Create<MoodReportJob>()
            .WithIdentity(key)
            .UsingJobData("data", JsonConvert.SerializeObject(new MoodReportJobData(options.UserId)))
            .Build();

        var time = _timeProvider.Now.NextDayOfWeek(DayOfWeek.Sunday, true)
            .WithTime(RemindersConsts.Evening730PM)
            .Add(-options.Gmt);

        var trigger = TriggerBuilder.Create()
            .ForJob(job)
            .WithCalendarIntervalSchedule(scheduler => scheduler.WithIntervalInWeeks(RemindersConsts.OneWeek))
            .StartAt(time)
            .Build();

        await scheduler.ScheduleJob(job, trigger, cancellationToken);
    }

    public Task Unschedule(IScheduler scheduler, MoodReportSchedulerOptions options, CancellationToken cancellationToken = default)
    {
        var key = JobKey.Create($"{nameof(MoodReportJob)}-{options.UserId}");
        return scheduler.DeleteJob(key, cancellationToken);
    }
}
