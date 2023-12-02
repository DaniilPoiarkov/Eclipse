using MediatR;

namespace Eclipse.Domain.Shared.Events;


/// <summary>
///   <para>Base class for all kind of domain events</para>
/// </summary>
public abstract class DomainEvent : INotification
{
}
