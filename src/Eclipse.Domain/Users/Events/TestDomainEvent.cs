using Eclipse.Common.Events;

namespace Eclipse.Domain.Users.Events;

[Serializable]
public sealed record TestDomainEvent : IDomainEvent
{
    public long ChatId { get; }

    public TestDomainEvent(long chatId)
    {
        ChatId = chatId;
    }
}
