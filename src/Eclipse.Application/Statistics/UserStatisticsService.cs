using Eclipse.Application.Contracts.Statistics;
using Eclipse.Domain.Statistics;

namespace Eclipse.Application.Statistics;

internal sealed class UserStatisticsService : IUserStatisticsService
{
    private readonly IUserStatisticsRepository _userStatisticsRepository;

    public UserStatisticsService(IUserStatisticsRepository userStatisticsRepository)
    {
        _userStatisticsRepository = userStatisticsRepository;
    }

    public async Task<UserStatisticsDto> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var statistics = await _userStatisticsRepository.FindByUserIdAsync(userId, cancellationToken);

        if (statistics is null)
        {
            return new UserStatisticsDto { UserId = userId };
        }

        return statistics.ToDto();
    }
}
