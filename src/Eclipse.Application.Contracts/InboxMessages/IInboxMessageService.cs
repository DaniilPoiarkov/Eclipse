namespace Eclipse.Application.Contracts.InboxMessages;

public interface IInboxMessageService
{
    Task DeleteProcessedAsync(CancellationToken cancellationToken = default);
}
