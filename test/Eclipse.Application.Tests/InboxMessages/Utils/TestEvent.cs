using Eclipse.Common.Events;

namespace Eclipse.Application.Tests.InboxMessages.Utils;

public sealed record TestEvent(Guid Data) : IDomainEvent;
