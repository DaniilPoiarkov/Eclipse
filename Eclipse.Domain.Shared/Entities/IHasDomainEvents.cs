using Eclipse.Domain.Shared.Events;

namespace Eclipse.Domain.Shared.Entities;

public interface IHasDomainEvents
{
    IReadOnlyList<IDomainEvent> GetEvents();
}
