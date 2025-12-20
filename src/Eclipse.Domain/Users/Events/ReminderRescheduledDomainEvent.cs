using Eclipse.Common.Events;

namespace Eclipse.Domain.Users.Events;

public sealed record ReminderRescheduledDomainEvent(Guid UserId, Guid ReminderId, TimeOnly NotifyAt) : IDomainEvent, IHasUserId;
