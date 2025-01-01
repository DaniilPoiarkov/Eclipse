using Eclipse.Common.Caching;
using Eclipse.DataAccess.Repositories;
using Eclipse.Domain.InboxMessages;

namespace Eclipse.DataAccess.InboxMessages;

internal sealed class CachedInboxMessageRepository : CachedRepositoryBase<InboxMessage, IInboxMessageRepository>, IInboxMessageRepository
{
    public CachedInboxMessageRepository(IInboxMessageRepository repository, ICacheService cacheService)
        : base(repository, cacheService) { }

    public Task DeleteSuccessfullyProcessedAsync(CancellationToken cancellationToken = default)
    {
        return Repository.DeleteSuccessfullyProcessedAsync(cancellationToken);
    }

    public Task<List<InboxMessage>> GetPendingAsync(int count, CancellationToken cancellationToken = default)
    {
        return CacheService.GetOrCreateAsync(
            $"{GetPrefix()}-pending-{count}",
            () => Repository.GetPendingAsync(count, cancellationToken),
            CacheConsts.FiveMinutes,
            cancellationToken
        );
    }

    public Task<List<InboxMessage>> GetPendingAsync(int count, string? handlerName, CancellationToken cancellationToken)
    {
        return CacheService.GetOrCreateAsync(
            $"{GetPrefix()}-pending-{handlerName}-{count}",
            () => Repository.GetPendingAsync(count, handlerName, cancellationToken),
            CacheConsts.FiveMinutes,
            cancellationToken
        );
    }
}
