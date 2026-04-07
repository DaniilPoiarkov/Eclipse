using Quartz;

namespace Eclipse.Application.Reminders.Sendings;

internal static class SchedulerExtensions
{
    internal static async Task<IJobDetail> GetSendReminderJob(this IScheduler scheduler, CancellationToken cancellationToken = default)
    {
        var job = await scheduler.GetJobDetail(SendReminderJob.Key, cancellationToken);

        job ??= JobBuilder.Create<SendReminderJob>()
            .WithIdentity(SendReminderJob.Key)
            .Build();
        
        return job;
    }
}
