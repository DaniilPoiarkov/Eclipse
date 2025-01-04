namespace Eclipse.Application.Contracts.OutboxMessages;

public interface IOutboxMessagesService
{
    Task DeleteSuccessfullyProcessedAsync(CancellationToken cancellationToken = default);
}
