using Eclipse.Common.Events;
using Eclipse.Domain.Shared.Users;

namespace Eclipse.Domain.Users.Events;

public sealed record TodoItemFinishedDomainEvent(Guid UserId) : IDomainEvent, IHasUserId;
