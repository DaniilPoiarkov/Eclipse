using Eclipse.Common.EventBus;
using Eclipse.Common.Events;

namespace Eclipse.Infrastructure.EventBus;

internal sealed class InMemoryEventBus : IEventBus
{
    private readonly InMemoryQueue<IDomainEvent> _queue;

    public InMemoryEventBus(InMemoryQueue<IDomainEvent> queue)
    {
        _queue = queue;
    }

    public ValueTask Publish<TEvent>(TEvent @event, CancellationToken cancellationToken = default)
        where TEvent : class, IDomainEvent
    {
        return _queue.WriteAsync(@event, cancellationToken);
    }
}
