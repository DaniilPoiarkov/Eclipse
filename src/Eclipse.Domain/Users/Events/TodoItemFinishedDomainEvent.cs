using Eclipse.Common.Events;

namespace Eclipse.Domain.Users.Events;

public sealed record TodoItemFinishedDomainEvent(Guid UserId) : IDomainEvent;
