using Eclipse.Common.Events;

namespace Eclipse.Domain.Users.Events;

public sealed record UserDisabledDomainEvent(Guid UserId) : IDomainEvent;

