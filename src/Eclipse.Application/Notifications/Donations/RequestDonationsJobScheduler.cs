using Eclipse.Application.Jobs;
using Eclipse.Common.Clock;
using Eclipse.Common.Notifications;

using Newtonsoft.Json;

using Quartz;

namespace Eclipse.Application.Notifications.Donations;

internal sealed class RequestDonationsJobScheduler : INotificationScheduler<RequestDonationsJob, SchedulerOptions>
{
    private static readonly int DayOfMonth = 9;

    private readonly ITimeProvider _timeProvider;

    public RequestDonationsJobScheduler(ITimeProvider timeProvider)
    {
        _timeProvider = timeProvider;
    }

    public async Task Schedule(IScheduler scheduler, SchedulerOptions options, CancellationToken cancellationToken = default)
    {
        var job = await scheduler.GetOrAddDurableJob<RequestDonationsJob>(cancellationToken);

        var time = _timeProvider.Now.NextMonth()
            .AddDays(DayOfMonth)
            .WithTime(NotificationConsts.Day130PM)
            .Add(-options.Gmt);

        if (time < _timeProvider.Now)
        {
            time = time.NextDay();
        }

        var trigger = TriggerBuilder.Create()
            .ForJob(job)
            .WithIdentity(new TriggerKey($"{nameof(RequestDonationsJob)}-{options.UserId}", options.UserId.ToString()))
            .UsingJobData("data", JsonConvert.SerializeObject(new UserIdJobData(options.UserId)))
            .WithCalendarIntervalSchedule(schedule =>
                schedule.WithIntervalInMonths(NotificationConsts.OneUnit)
            )
            .StartAt(time)
            .Build();

        await scheduler.ScheduleJob(trigger, cancellationToken);
    }

    public Task Unschedule(IScheduler scheduler, SchedulerOptions options, CancellationToken cancellationToken = default)
    {
        return scheduler.UnscheduleJob(
            new TriggerKey($"{nameof(RequestDonationsJob)}-{options.UserId}", options.UserId.ToString()),
            cancellationToken
        );
    }
}
