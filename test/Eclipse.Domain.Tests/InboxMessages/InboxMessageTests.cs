using Eclipse.Domain.InboxMessages;
using Eclipse.Domain.Shared.InboxMessages;

using FluentAssertions;

using Xunit;

namespace Eclipse.Domain.Tests.InboxMessages;

public sealed class InboxMessageTests
{
    [Fact]
    public void Reset_WhenInFailedState_ThenResetsStatus()
    {
        var inboxMessage = InboxMessage.Create(Guid.NewGuid(), Guid.NewGuid(), "handler", "{}", "object", DateTime.UtcNow);

        var error = "An error occurred";
        var processedAt = DateTime.UtcNow;

        inboxMessage.SetError(error, processedAt);

        inboxMessage.Reset();

        inboxMessage.Status.Should().Be(InboxMessageStatus.Pending);
        inboxMessage.Error.Should().Be(error);
        inboxMessage.ProcessedAt.Should().Be(processedAt);
    }

    [Fact]
    public void Reset_WhenNotFailed_ThenExceptionThrown()
    {
        var inboxMessage = InboxMessage.Create(Guid.NewGuid(), Guid.NewGuid(), "handler", "{}", "object", DateTime.UtcNow);
        
        inboxMessage.Invoking(im => im.Reset())
            .Should().Throw<InvalidOperationException>()
            .WithMessage("Cannot reset a message that is not in Failed status.");
    }
}
