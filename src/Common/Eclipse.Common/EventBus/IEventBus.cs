using Eclipse.Common.Events;

namespace Eclipse.Common.EventBus;

public interface IEventBus
{
    Task Publish<TEvent>(TEvent @event, CancellationToken cancellationToken = default)
        where TEvent : class, IDomainEvent;
}
