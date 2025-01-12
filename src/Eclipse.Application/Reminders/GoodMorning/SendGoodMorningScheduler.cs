using Eclipse.Common.Clock;

using Newtonsoft.Json;

using Quartz;

namespace Eclipse.Application.Reminders.GoodMorning;

internal sealed class SendGoodMorningScheduler : IJobScheduler<SendGoodMorningJob, SendGoodMorningSchedulerOptions>
{
    private readonly ITimeProvider _timeProvider;

    public SendGoodMorningScheduler(ITimeProvider timeProvider)
    {
        _timeProvider = timeProvider;
    }

    public async Task Schedule(IScheduler scheduler, SendGoodMorningSchedulerOptions options, CancellationToken cancellationToken = default)
    {
        var key = JobKey.Create($"{nameof(SendGoodMorningJob)}-{options.UserId}");

        var job = JobBuilder.Create<SendGoodMorningJob>()
            .WithIdentity(key)
            .UsingJobData("data", JsonConvert.SerializeObject(new SendGoodMorningJobData(options.UserId)))
            .Build();

        var time = _timeProvider.Now.WithTime(RemindersConsts.Morning9AM)
            .Add(options.Gmt);

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
