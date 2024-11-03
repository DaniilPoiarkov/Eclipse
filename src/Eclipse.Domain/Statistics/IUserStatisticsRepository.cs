using Eclipse.Domain.Shared.Repositories;

namespace Eclipse.Domain.Statistics;

public interface IUserStatisticsRepository : IRepository<UserStatistics>
{
    Task<UserStatistics?> FindByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
}
