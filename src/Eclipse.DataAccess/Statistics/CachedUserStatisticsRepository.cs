using Eclipse.Common.Caching;
using Eclipse.DataAccess.Repositories;
using Eclipse.Domain.Statistics;

namespace Eclipse.DataAccess.Statistics;

internal sealed class CachedUserStatisticsRepository : CachedRepositoryBase<UserStatistics, IUserStatisticsRepository>, IUserStatisticsRepository
{
    public CachedUserStatisticsRepository(
        IUserStatisticsRepository repository,
        ICacheService cacheService) : base(repository, cacheService) { }

    public async Task<UserStatistics?> FindByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var key = $"user-statistics-{userId}";
        var statistics = await CacheService.GetAsync<UserStatistics>(key, cancellationToken);

        if (statistics is not null)
        {
            return statistics;
        }

        statistics = await Repository.FindByUserIdAsync(userId, cancellationToken);

        if (statistics is not null)
        {
            await CacheService.SetAsync(key, statistics, CacheConsts.OneDay, cancellationToken);
        }

        return statistics;
    }
}
