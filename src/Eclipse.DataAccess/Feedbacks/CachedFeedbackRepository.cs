using Eclipse.Common.Caching;
using Eclipse.DataAccess.Repositories;
using Eclipse.DataAccess.Repositories.Caching;
using Eclipse.Domain.Feedbacks;

namespace Eclipse.DataAccess.Feedbacks;

internal sealed class CachedFeedbackRepository : CachedRepositoryBase<Feedback, IFeedbackRepository>, IFeedbackRepository
{
    public CachedFeedbackRepository(
        IFeedbackRepository repository,
        ICacheService cacheService,
        IExpressionHashStore expressionHashStore)
        : base(repository, cacheService, expressionHashStore) { }
}
