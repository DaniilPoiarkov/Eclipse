using Eclipse.IntegrationEvents.Users;

using MassTransit;

using Microsoft.Extensions.Logging;

using System.Text.Json;

namespace Eclipse.Notifications.Users.NewUserRegistered;

public sealed class ScheduleJobs : IConsumer<NewUserRegisteredIntegrationEvent>
{
    private readonly ILogger<ScheduleJobs> _logger;

    public ScheduleJobs(ILogger<ScheduleJobs> logger)
    {
        _logger = logger;
    }

    public Task Consume(ConsumeContext<NewUserRegisteredIntegrationEvent> context)
    {
        _logger.LogInformation("Handler {Handler} with data {Data}", nameof(ScheduleJobs), JsonSerializer.Serialize(context.Message, new JsonSerializerOptions
        {
            WriteIndented = true,
        }));

        return Task.CompletedTask;
    }
}
