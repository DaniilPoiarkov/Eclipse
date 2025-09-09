using Eclipse.Common.Events;
using Eclipse.Domain.Shared.Users;

namespace Eclipse.Domain.Users.Events;

public sealed record UserEnabledDomainEvent(Guid UserId) : IDomainEvent, IHasUserId;
