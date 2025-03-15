using Eclipse.DataAccess.Cosmos;
using Eclipse.DataAccess.Repositories;
using Eclipse.Domain.Statistics;

using Microsoft.EntityFrameworkCore;

namespace Eclipse.DataAccess.Statistics;

internal sealed class UserStatisticsRepository : RepositoryBase<UserStatistics>, IUserStatisticsRepository
{
    public UserStatisticsRepository(EclipseDbContext context)
        : base(context) { }

    public Task<UserStatistics?> FindByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return DbSet.FirstOrDefaultAsync(us => us.UserId == userId, cancellationToken);
    }
}
