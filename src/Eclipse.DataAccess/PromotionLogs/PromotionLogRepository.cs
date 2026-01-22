using Eclipse.DataAccess.Cosmos;
using Eclipse.DataAccess.Repositories;
using Eclipse.Domain.PromotionLogs;

namespace Eclipse.DataAccess.PromotionLogs;

internal sealed class PromotionLogRepository : RepositoryBase<PromotionLog>, IPromotionLogRepository
{
    public PromotionLogRepository(EclipseDbContext context)
        : base(context) { }
}
