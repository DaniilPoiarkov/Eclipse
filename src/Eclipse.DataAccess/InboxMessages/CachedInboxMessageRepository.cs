using Eclipse.Common.Caching;
using Eclipse.DataAccess.Repositories;
using Eclipse.Domain.InboxMessages;

namespace Eclipse.DataAccess.InboxMessages;

internal sealed class CachedInboxMessageRepository : CachedRepositoryBase<InboxMessage, IInboxMessageRepository>, IInboxMessageRepository
{
    public CachedInboxMessageRepository(IInboxMessageRepository repository, ICacheService cacheService)
        : base(repository, cacheService) { }

    public async Task DeleteSuccessfullyProcessedAsync(CancellationToken cancellationToken = default)
    {
        await CacheService.DeleteByPrefixAsync(GetPrefix(), cancellationToken);
        await Repository.DeleteSuccessfullyProcessedAsync(cancellationToken);
    }

    public Task<List<InboxMessage>> GetPendingAsync(int count, string? handlerName, CancellationToken cancellationToken = default)
    {
        var options = new CacheOptions
        {
            Expiration = CacheConsts.FiveMinutes,
            Tags = [GetPrefix()]
        };

        return CacheService.GetOrCreateAsync(
            $"{GetPrefix()}-pending-{handlerName}-{count}",
            () => Repository.GetPendingAsync(count, handlerName, cancellationToken),
            options,
            cancellationToken
        );
    }
}
