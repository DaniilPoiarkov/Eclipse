using Eclipse.Common.Caching;
using Eclipse.DataAccess.Repositories;
using Eclipse.DataAccess.Repositories.Caching;
using Eclipse.Domain.PromotionLogs;

namespace Eclipse.DataAccess.PromotionLogs;

internal sealed class CachedPromotionLogRepository : CachedRepositoryBase<PromotionLog, PromotionLogRepository>, IPromotionLogRepository
{
    public CachedPromotionLogRepository(
        PromotionLogRepository repository,
        ICacheService cacheService,
        IExpressionHashStore expressionHashStore)
        : base(repository, cacheService, expressionHashStore) { }
}
