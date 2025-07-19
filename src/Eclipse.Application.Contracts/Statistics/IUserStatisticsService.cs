namespace Eclipse.Application.Contracts.Statistics;

public interface IUserStatisticsService
{
    Task<UserStatisticsDto> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
}
