using Eclipse.Common.Results;

namespace Eclipse.Application.Contracts.Statistics;

public interface IUserStatisticsService
{
    Task<Result<UserStatisticsDto>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
}
