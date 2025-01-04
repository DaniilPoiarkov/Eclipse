using Eclipse.Common.Events;

namespace Eclipse.Application.Tests.InboxMessages.Utils;

public sealed class AnotherTestEventHandler : IEventHandler<TestEvent>
{
    public Task Handle(TestEvent @event, CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }
}
