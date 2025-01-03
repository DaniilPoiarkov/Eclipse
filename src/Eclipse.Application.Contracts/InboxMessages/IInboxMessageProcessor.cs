using Eclipse.Common.Events;

namespace Eclipse.Application.Contracts.InboxMessages;

public interface IInboxMessageProcessor
{
    Task<ProcessInboxMessagesResult> ProcessAsync(int count, CancellationToken cancellationToken = default);
}

public interface IInboxMessageProcessor<TEvent, TEventHandler> : IInboxMessageProcessor
    where TEvent : IDomainEvent
    where TEventHandler : IEventHandler<TEvent>
{
}
