using Eclipse.Common.Events;
using Eclipse.Domain.Users;

namespace Eclipse.Domain.Reminders;

public sealed record ReminderRescheduledDomainEvent(Guid UserId, Guid ReminderId, TimeOnly NotifyAt) : IDomainEvent, IHasUserId;
