namespace Eclipse.Common.Events;

public interface IEventHandler<in TEvent>
    where TEvent : IDomainEvent
{
    Task Handle(TEvent enevt, CancellationToken cancellationToken = default);
}
