using Eclipse.Common.Caching;
using Eclipse.DataAccess.Repositories;
using Eclipse.DataAccess.Repositories.Caching;
using Eclipse.Domain.Statistics;

namespace Eclipse.DataAccess.Statistics;

internal sealed class CachedUserStatisticsRepository : CachedRepositoryBase<UserStatistics, IUserStatisticsRepository>, IUserStatisticsRepository
{
    public CachedUserStatisticsRepository(
        IUserStatisticsRepository repository,
        ICacheService cacheService,
        IExpressionHashStore expressionHashStore) : base(repository, cacheService, expressionHashStore) { }

    public Task<UserStatistics?> FindByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var options = new CacheOptions
        {
            Expiration = CacheConsts.OneDay,
            Tags = [GetPrefix()]
        };

        return CacheService.GetOrCreateAsync(
            $"{GetPrefix()}-user-id-{userId}",
            () => Repository.FindByUserIdAsync(userId, cancellationToken),
            options,
            cancellationToken
        );
    }
}
