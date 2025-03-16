using Eclipse.Application.Reminders.Core;
using Eclipse.Common.Clock;

using Newtonsoft.Json;

using Quartz;

namespace Eclipse.Application.Reminders.GoodMorning;

internal sealed class GoodMorningScheduler : IJobScheduler<GoodMorningJob, GoodMorningSchedulerOptions>
{
    private readonly ITimeProvider _timeProvider;

    public GoodMorningScheduler(ITimeProvider timeProvider)
    {
        _timeProvider = timeProvider;
    }

    public async Task Schedule(IScheduler scheduler, GoodMorningSchedulerOptions options, CancellationToken cancellationToken = default)
    {
        var key = JobKey.Create($"{nameof(GoodMorningJob)}-{options.UserId}");

        var job = JobBuilder.Create<GoodMorningJob>()
            .WithIdentity(key)
            .UsingJobData("data", JsonConvert.SerializeObject(new GoodMorningJobData(options.UserId)))
            .Build();

        var time = _timeProvider.Now.WithTime(RemindersConsts.Morning9AM)
            .Add(-options.Gmt);

        if (time < _timeProvider.Now)
        {
            time = time.NextDay();
        }

        var trigger = TriggerBuilder.Create()
            .ForJob(job)
            .WithSimpleSchedule(schedule => schedule
                .WithIntervalInHours(RemindersConsts.OneDayInHours)
                .RepeatForever()
            )
            .StartAt(time)
            .Build();

        await scheduler.ScheduleJob(job, trigger, cancellationToken);
    }

    public Task Unschedule(IScheduler scheduler, GoodMorningSchedulerOptions options, CancellationToken cancellationToken = default)
    {
        var key = JobKey.Create($"{nameof(GoodMorningJob)}-{options.UserId}");
        return scheduler.DeleteJob(key, cancellationToken);
    }
}
