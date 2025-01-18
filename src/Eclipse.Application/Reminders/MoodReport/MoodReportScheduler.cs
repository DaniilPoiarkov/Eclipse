using Eclipse.Common.Clock;

using Newtonsoft.Json;

using Quartz;

namespace Eclipse.Application.Reminders.MoodReport;

internal sealed class MoodReportScheduler : IJobScheduler<SendMoodReportJob, MoodReportSchedulerOptions>
{
    private readonly ITimeProvider _timeProvider;

    public MoodReportScheduler(ITimeProvider timeProvider)
    {
        _timeProvider = timeProvider;
    }

    public async Task Schedule(IScheduler scheduler, MoodReportSchedulerOptions options, CancellationToken cancellationToken = default)
    {
        var key = JobKey.Create($"{nameof(SendMoodReportJob)}-{options.UserId}");

        var job = JobBuilder.Create<SendMoodReportJob>()
            .WithIdentity(key)
            .UsingJobData("data", JsonConvert.SerializeObject(new SendMoodReportJobData(options.UserId)))
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
}
