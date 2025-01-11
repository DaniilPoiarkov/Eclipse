using Eclipse.Common.Events;

namespace Eclipse.Domain.Users.Events;

public sealed record GmtChangedDomainEvent(Guid UserId, TimeSpan Gmt) : IDomainEvent;
