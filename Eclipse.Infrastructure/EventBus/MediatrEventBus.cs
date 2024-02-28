using Eclipse.Common.EventBus;

using MediatR;

using Microsoft.Extensions.DependencyInjection;

namespace Eclipse.Infrastructure.EventBus;

internal sealed class MediatrEventBus : IEventBus
{
    private readonly IPublisher _publisher;

    public MediatrEventBus(IServiceProvider services)
    {
        _publisher = services.CreateScope()
            .ServiceProvider
            .GetRequiredService<IPublisher>();
    }

    public Task Publish<TEvent>(TEvent @event, CancellationToken cancellationToken = default)
        where TEvent : class
    {
        return _publisher.Publish(@event, cancellationToken);
    }
}
