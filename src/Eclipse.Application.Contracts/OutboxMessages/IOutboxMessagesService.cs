namespace Eclipse.Application.Contracts.OutboxMessages;

public interface IOutboxMessagesService
{
    Task<ProcessOutboxMessagesResult> ProcessAsync(int count, CancellationToken cancellationToken = default);

    Task DeleteSuccessfullyProcessedAsync(CancellationToken cancellationToken = default);
}
