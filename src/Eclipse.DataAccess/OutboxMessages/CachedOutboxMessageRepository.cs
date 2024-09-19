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
        await CacheService.DeleteByPrefixAsync(GetPrefix(), cancellationToken);
        await Repository.DeleteSuccessfullyProcessedAsync(cancellationToken);
    }

    public async Task<List<OutboxMessage>> GetNotProcessedAsync(int count, CancellationToken cancellationToken = default)
    {
        var key = new CacheKey($"{GetPrefix()}-not-processed-{count}");
        var outboxMessages = await CacheService.GetAsync<List<OutboxMessage>>(key, cancellationToken);

        if (outboxMessages is not null)
        {
            return outboxMessages;
        }

        outboxMessages = await Repository.GetNotProcessedAsync(count, cancellationToken);

        await CacheService.SetAsync(key, outboxMessages, CacheConsts.FiveMinutes, cancellationToken);

        return outboxMessages;
    }
}
