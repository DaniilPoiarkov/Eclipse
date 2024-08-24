using Eclipse.Common.Caching;
using Eclipse.DataAccess.Repositories;
using Eclipse.Domain.OutboxMessages;

namespace Eclipse.DataAccess.OutboxMessages;

internal sealed class CachedOutboxMessageRepository : CachedRepositoryBase<OutboxMessage, IOutboxMessageRepository>, IOutboxMessageRepository
{
    public CachedOutboxMessageRepository(IOutboxMessageRepository repository, ICacheService cacheService)
        : base(repository, cacheService) { }

    public async Task DeleteSuccessfullyProcessedAsync(CancellationToken cancellationToken = default)
    {
        await RemoveByPrefixAsync(cancellationToken);
        await Repository.DeleteSuccessfullyProcessedAsync(cancellationToken);
    }

    public Task<List<OutboxMessage>> GetNotProcessedAsync(int count, CancellationToken cancellationToken = default)
    {
        return Repository.GetNotProcessedAsync(count, cancellationToken);
    }
}
