using Eclipse.Domain.Shared.Repositories;

namespace Eclipse.Domain.InboxMessages;

public interface IInboxMessageRepository : IRepository<InboxMessage>
{
    Task<List<InboxMessage>> GetPendingAsync(int count, CancellationToken cancellationToken = default);

    Task<List<InboxMessage>> GetPendingAsync(int count, string? handlerName, CancellationToken cancellationToken);

    Task DeleteSuccessfullyProcessedAsync(CancellationToken cancellationToken = default);
}
