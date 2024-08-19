using Eclipse.Common.Events;

namespace Eclipse.Domain.Users.Events;

public sealed record RemindersRecievedDomainEvent(Guid UserId, int Count) : IDomainEvent;
