using Eclipse.Common.Events;

using MediatR;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Eclipse.Infrastructure.EventBus;

internal sealed class InMemoryChannelReadService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;

    private readonly InMemoryQueue<IDomainEvent> _queue;

    private readonly ILogger<InMemoryChannelReadService> _logger;

    public InMemoryChannelReadService(IServiceScopeFactory scopeFactory, InMemoryQueue<IDomainEvent> queue, ILogger<InMemoryChannelReadService> logger)
    {
        _scopeFactory = scopeFactory;
        _queue = queue;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Start listening {service} background service.", nameof(InMemoryChannelReadService));

        while (!stoppingToken.IsCancellationRequested)
        {
            await foreach (var @event in _queue.ReadAsync(stoppingToken))
            {
                using var scope = _scopeFactory.CreateAsyncScope();

                await HandleEvent(@event, scope.ServiceProvider, stoppingToken);
            }
        }
    }

    private async Task HandleEvent(IDomainEvent @event, IServiceProvider services, CancellationToken cancellationToken)
    {
        var publisher = services.GetRequiredService<IPublisher>();

        try
        {
            await publisher.Publish(@event, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during publishing event:\n\r{error}\n\r{trace}", ex.Message, ex.StackTrace);
        }
    }
}
