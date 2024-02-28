using MediatR;

namespace Eclipse.Domain.Shared.Events;

/// <summary>
///   <para>Marker for all kind of domain events</para>
/// </summary>
public interface IDomainEvent : INotification
{
}
