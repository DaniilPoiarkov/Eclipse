namespace Eclipse.Common.Events;

public interface IEventHandler<TEvent>
    where TEvent : IDomainEvent
{
    Task Handle(TEvent enevt, CancellationToken cancellationToken = default);
}
