using Eclipse.Common.Events;

namespace Eclipse.Tests.OutboxMessages;

public sealed class TestOutboxMessageDomainEvent : IDomainEvent
{
    public string Text { get; }

    public TestOutboxMessageDomainEvent(string text)
    {
        Text = text;
    }
}
