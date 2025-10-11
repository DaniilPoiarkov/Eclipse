using Eclipse.DataAccess.Cosmos;
using Eclipse.DataAccess.Repositories;
using Eclipse.Domain.Feedbacks;

namespace Eclipse.DataAccess.Feedbacks;

internal sealed class FeedbackRepository : RepositoryBase<Feedback>, IFeedbackRepository
{
    public FeedbackRepository(EclipseDbContext context) : base(context)
    {
    }
}
