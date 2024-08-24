using Eclipse.Common.Events;

using MediatR;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using Newtonsoft.Json;

using StackExchange.Redis;

namespace Eclipse.Infrastructure.EventBus.Redis;

internal sealed class RedisChannelReadService : BackgroundService
{
    private readonly IServiceScopeFactory _serviceScopeFactory;

    private readonly ILogger<RedisChannelReadService> _logger;

    public RedisChannelReadService(IServiceScopeFactory serviceScopeFactory, ILogger<RedisChannelReadService> logger)
    {
        _serviceScopeFactory = serviceScopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Start listening {service} background service.", nameof(RedisChannelReadService));

        using var scope = _serviceScopeFactory.CreateAsyncScope();

        var connectionMultiplexer = scope.ServiceProvider.GetRequiredService<IConnectionMultiplexer>();
        var subscriber = connectionMultiplexer.GetSubscriber();

        var queue = await subscriber.SubscribeAsync(RedisChannel.Literal("domain-events"));

        while (!stoppingToken.IsCancellationRequested)
        {
            if (!queue.TryRead(out var message))
            {
                continue;
            }

            if (!message.Message.HasValue)
            {
                _logger.LogWarning("Domain event message is null. Message: {message}", message.Message);
                continue;
            }

            await HandleEventAsync(scope, message.Message!, stoppingToken);
        }
    }

    private async Task HandleEventAsync(AsyncServiceScope scope, string message, CancellationToken cancellationToken)
    {
        var publisher = scope.ServiceProvider.GetRequiredService<IPublisher>();

        try
        {
            var @event = JsonConvert.DeserializeObject<IDomainEvent>(message, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.All
            });

            if (@event is null)
            {
                _logger.LogError("Error during publishing event:\n\r{error}", "event is null");
                return;
            }

            await publisher.Publish(@event, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during publishing event:\n\r{error}", ex.Message);
        }
    }
}
