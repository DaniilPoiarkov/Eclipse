using Eclipse.Domain.Events;

namespace Eclipse.Domain.Behaviors;

public interface IHasDomainEvents
{
    IReadOnlyList<IDomainEvent> GetEvents();
}
