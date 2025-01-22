using Eclipse.Application.Reminders.Core;
using Eclipse.Common.Clock;

using Newtonsoft.Json;

using Quartz;

namespace Eclipse.Application.Reminders.GoodMorning;

internal sealed class GoodMorningScheduler : IJobScheduler<RegularJob<GoodMorningJob, GoodMorningJobData>, GoodMorningSchedulerOptions>
{
    private readonly ITimeProvider _timeProvider;

    public GoodMorningScheduler(ITimeProvider timeProvider)
    {
        _timeProvider = timeProvider;
    }

    public async Task Schedule(IScheduler scheduler, GoodMorningSchedulerOptions options, CancellationToken cancellationToken = default)
    {
        var key = JobKey.Create($"{nameof(GoodMorningJob)}-{options.UserId}");

        var job = JobBuilder.Create<RegularJob<GoodMorningJob, GoodMorningJobData>>()
            .WithIdentity(key)
            .UsingJobData("data", JsonConvert.SerializeObject(new GoodMorningJobData(options.UserId)))
            .Build();

        var time = _timeProvider.Now.WithTime(RemindersConsts.Morning9AM)
            .Add(-options.Gmt);

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
}
