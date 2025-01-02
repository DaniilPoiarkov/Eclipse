using Eclipse.Common.Events;

namespace Eclipse.Application.Tests.InboxMessages.Utils;

internal sealed class TestEventHandler : IEventHandler<TestEvent>
{
    public Task Handle(TestEvent @event, CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }
}
