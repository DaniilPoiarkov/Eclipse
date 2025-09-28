using Eclipse.Common.Events;

namespace Eclipse.Domain.Users.Events;

public sealed record UserEnabledDomainEvent(Guid UserId) : IDomainEvent, IHasUserId;
