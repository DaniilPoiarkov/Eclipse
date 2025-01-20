using Microsoft.Extensions.Logging;

using Newtonsoft.Json;

using Quartz;

namespace Eclipse.Application.Reminders.Core.NewUserJoined;

internal sealed class NewUserJoinedJob<TNotificationJob, TArgs> : IJob
    where TNotificationJob : INotificationJob<TArgs>
{
    private readonly ILogger<NewUserJoinedJob<TNotificationJob, TArgs>> _logger;

    private readonly TNotificationJob _job;

    private const int _maxRefireCount = 5;

    public NewUserJoinedJob(ILogger<NewUserJoinedJob<TNotificationJob, TArgs>> logger, TNotificationJob job)
    {
        _logger = logger;
        _job = job;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        if (context.RefireCount >= _maxRefireCount)
        {
            _logger.LogError("Failed to process {Job} job. Max refire count exceeded", typeof(TNotificationJob).Name);
            return;
        }

        var data = context.MergedJobDataMap.GetString("data");

        if (data.IsNullOrEmpty())
        {
            _logger.LogError("Cannot deserialize event with data {Data}", "{null}");
            return;
        }

        var args = JsonConvert.DeserializeObject<TArgs>(data);

        if (args is null)
        {
            _logger.LogError("Cannot deserialize event with data {Data}", data);
            return;
        }

        try
        {
            await _job.Handle(args, context.CancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to process {Job} job.", typeof(TNotificationJob).Name);

            throw new JobExecutionException($"Failed to process {typeof(TNotificationJob).Name} job.", ex, true);
        }
    }
}
