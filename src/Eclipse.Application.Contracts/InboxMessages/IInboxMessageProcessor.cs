namespace Eclipse.Application.Contracts.InboxMessages;

public interface IInboxMessageProcessor
{
    Task<ProcessInboxMessagesResult> ProcessAsync(int count, CancellationToken cancellationToken = default);
}
