using Eclipse.Common.Clock;
using Eclipse.Common.Events;
using Eclipse.Domain.Users.Events;

using Newtonsoft.Json;

using Quartz;

namespace Eclipse.Application.Reminders.GoodMorning;

internal sealed class RescheduleForNewTimeSendGoodMorningHandler : IEventHandler<GmtChangedDomainEvent>
{
    private readonly ISchedulerFactory _schedulerFactory;

    private readonly ITimeProvider _timeProvider;

    public RescheduleForNewTimeSendGoodMorningHandler(ISchedulerFactory schedulerFactory, ITimeProvider timeProvider)
    {
        _schedulerFactory = schedulerFactory;
        _timeProvider = timeProvider;
    }

    public async Task Handle(GmtChangedDomainEvent @event, CancellationToken cancellationToken = default)
    {
        var key = JobKey.Create($"{nameof(SendGoodMorningJob)}-{@event.UserId}");

        var scheduler = await _schedulerFactory.GetScheduler(cancellationToken);

        await scheduler.DeleteJob(key, cancellationToken);

        var job = JobBuilder.Create<SendGoodMorningJob>()
            .WithIdentity(key)
            .UsingJobData("data", JsonConvert.SerializeObject(new SendGoodMorningJobData(@event.UserId)))
            .Build();

        var time = _timeProvider.Now.WithTime(RemindersConsts.Morning9AM)
            .Add(@event.Gmt);

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
