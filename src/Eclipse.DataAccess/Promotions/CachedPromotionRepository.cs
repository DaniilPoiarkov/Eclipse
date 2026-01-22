using Eclipse.Common.Caching;
using Eclipse.DataAccess.Repositories;
using Eclipse.DataAccess.Repositories.Caching;
using Eclipse.Domain.Promotions;

namespace Eclipse.DataAccess.Promotions;

internal sealed class CachedPromotionRepository : CachedRepositoryBase<Promotion, PromotionRepository>, IPromotionRepository
{
    public CachedPromotionRepository(
        PromotionRepository repository,
        ICacheService cacheService,
        IExpressionHashStore expressionHashStore)
        : base(repository, cacheService, expressionHashStore) { }
}
