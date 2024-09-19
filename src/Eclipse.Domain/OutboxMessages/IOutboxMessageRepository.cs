using Eclipse.Domain.Shared.Repositories;

namespace Eclipse.Domain.OutboxMessages;

public interface IOutboxMessageRepository : IRepository<OutboxMessage>
{
    Task<List<OutboxMessage>> GetNotProcessedAsync(int count, CancellationToken cancellationToken = default);

    Task DeleteSuccessfullyProcessedAsync(CancellationToken cancellationToken = default);
}
