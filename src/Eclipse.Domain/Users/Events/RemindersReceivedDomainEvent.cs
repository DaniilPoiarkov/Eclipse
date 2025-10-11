using Eclipse.Common.Events;

namespace Eclipse.Domain.Users.Events;

public sealed record RemindersReceivedDomainEvent(Guid UserId) : IDomainEvent, IHasUserId;
