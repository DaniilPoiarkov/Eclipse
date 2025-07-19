namespace Eclipse.Common.Events;

public interface IEventHandler<TEvent>
    where TEvent : IDomainEvent
{
    Task Handle(TEvent @event, CancellationToken cancellationToken = default);
}
