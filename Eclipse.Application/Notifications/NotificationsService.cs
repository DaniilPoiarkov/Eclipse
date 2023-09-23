using Eclipse.Application.Contracts.Notifications;
using Eclipse.Application.Quartz.JobConfigurations;
using Eclipse.Infrastructure.Quartz;

namespace Eclipse.Application.Notifications;

internal class NotificationsService : INotificationService
{
    private readonly IEclipseScheduler _scheduler;

    public NotificationsService(IEclipseScheduler scheduler)
    {
        _scheduler = scheduler;
    }

    public Task AddJob()
    {
        return Task.CompletedTask;
    }

    public async Task AddTestJob(long id, TimeZoneInfo timeZone)
    {
        var options = new TestJobConfigurationOptions(id, timeZone, 19, 0);
        var configuration = new TestJobConfiguration(options);

        await _scheduler.ScheduleJob(configuration);
    }

    public Task DeleteJob(string group)
    {
        throw new NotImplementedException();
    }
}
