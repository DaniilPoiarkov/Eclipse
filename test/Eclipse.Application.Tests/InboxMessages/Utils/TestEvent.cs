using Eclipse.Common.Events;

namespace Eclipse.Application.Tests.InboxMessages.Utils;

internal sealed record TestEvent(Guid Data) : IDomainEvent;
