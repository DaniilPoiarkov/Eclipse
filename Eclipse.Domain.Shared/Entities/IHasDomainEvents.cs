using Eclipse.Common.Events;

namespace Eclipse.Domain.Shared.Entities;

public interface IHasDomainEvents
{
    IReadOnlyList<IDomainEvent> GetEvents();
}
