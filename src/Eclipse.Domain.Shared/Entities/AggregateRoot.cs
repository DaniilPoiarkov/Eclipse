using Eclipse.Common.Events;

namespace Eclipse.Domain.Shared.Entities;

public abstract class AggregateRoot : Entity, IHasDomainEvents
{
    private readonly List<IDomainEvent> _domainEvents = [];

    public AggregateRoot(Guid id)
        : base(id) { }

    public AggregateRoot() { }

    /// <summary>
    /// Adds the event.
    /// </summary>
    /// <param name="domainEvent">The domain event.</param>
    /// <returns></returns>
    protected void AddEvent(IDomainEvent domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }

    /// <summary>
    /// Gets the events.
    /// </summary>
    /// <returns></returns>
    public IReadOnlyList<IDomainEvent> GetEvents(bool clear = true)
    {
        var events = _domainEvents.ToList();

        if (clear)
        {
            _domainEvents.Clear();
        }

        return events;
    }
}
