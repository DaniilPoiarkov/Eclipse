using Eclipse.Common.Events;

namespace Eclipse.Domain.Users.Events;

public sealed record ReminderRemovedDomainEvent(Guid UserId, Guid ReminderId) : IHasUserId, IDomainEvent;
