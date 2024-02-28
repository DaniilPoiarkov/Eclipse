using Eclipse.Common.EventBus;

using MediatR;

namespace Eclipse.Infrastructure.EventBus;

internal sealed class MediatrEventBus : IEventBus
{
    private readonly IPublisher _publisher;

    public MediatrEventBus(IPublisher publisher)
    {
        _publisher = publisher;
    }

    public Task Publish<TEvent>(TEvent @event, CancellationToken cancellationToken = default)
        where TEvent : class
    {
        return _publisher.Publish(@event, cancellationToken);
    }
}
