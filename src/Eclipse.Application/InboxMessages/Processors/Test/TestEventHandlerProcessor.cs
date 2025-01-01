using Eclipse.Application.Users.EventHandlers;
using Eclipse.Domain.Users.Events;

using Microsoft.Extensions.Configuration;

namespace Eclipse.Application.InboxMessages.Processors.Test;

internal sealed class TestEventHandlerProcessorJob : ProcessTypedInboxMessagesJob<TestDomainEvent, TestEventHandler>
{
    public TestEventHandlerProcessorJob(
        TypedInboxMessageProcessor<TestDomainEvent, TestEventHandler> processor,
        IConfiguration configuration)
        : base(processor, configuration) { }
}

internal sealed class AnotherTestEventHandlerProcessorJob : ProcessTypedInboxMessagesJob<TestDomainEvent, AnotherTestEventHandler>
{
    public AnotherTestEventHandlerProcessorJob(
        TypedInboxMessageProcessor<TestDomainEvent, AnotherTestEventHandler> processor,
        IConfiguration configuration)
        : base(processor, configuration) { }
}
