using Eclipse.Common.Events;

namespace Eclipse.Domain.Users.Events;

internal sealed record ReminderRemovedDomainEvent(Guid UserId, Guid ReminderId) : IHasUserId, IDomainEvent;
