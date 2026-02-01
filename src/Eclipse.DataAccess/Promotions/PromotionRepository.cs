using Eclipse.DataAccess.Cosmos;
using Eclipse.DataAccess.Repositories;
using Eclipse.Domain.Promotions;

namespace Eclipse.DataAccess.Promotions;

internal sealed class PromotionRepository : RepositoryBase<Promotion>, IPromotionRepository
{
    public PromotionRepository(EclipseDbContext context)
        : base(context) { }
}
