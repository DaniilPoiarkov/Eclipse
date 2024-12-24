using Eclipse.IntegrationEvents.Users;

using MassTransit;

using Microsoft.Extensions.Logging;

using System.Text.Json;

namespace Eclipse.Notifications.Users.NewUserRegistered;

public sealed class TestHandler : IConsumer<NewUserRegisteredIntegrationEvent>
{
    private readonly ILogger<TestHandler> _logger;

    private static readonly JsonSerializerOptions _options = new()
    {
        WriteIndented = true,
    };

    public TestHandler(ILogger<TestHandler> logger)
    {
        _logger = logger;
    }

    public Task Consume(ConsumeContext<NewUserRegisteredIntegrationEvent> context)
    {
        _logger.LogInformation("Handler {Handler} with data {Data}", nameof(TestHandler), JsonSerializer.Serialize(context.Message, _options));

        return Task.CompletedTask;
    }
}
