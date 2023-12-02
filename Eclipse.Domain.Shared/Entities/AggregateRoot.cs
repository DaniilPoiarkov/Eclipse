using Eclipse.Domain.Shared.Events;

namespace Eclipse.Domain.Shared.Entities;

public abstract class AggregateRoot : Entity
{
    private readonly List<DomainEvent> _domainEvents = [];

    public AggregateRoot(Guid id)
        : base(id) { }

    public AggregateRoot() { }

    /// <summary>
    /// Adds the event.
    /// </summary>
    /// <param name="domainEvent">The domain event.</param>
    /// <returns></returns>
    protected void AddEvent(DomainEvent domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }

    /// <summary>
    /// Gets the events.
    /// </summary>
    /// <returns></returns>
    public IReadOnlyList<DomainEvent> GetEvents()
    {
        return _domainEvents.AsReadOnly();
    }
}
