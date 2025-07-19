using Eclipse.Application.Contracts.Statistics;
using Eclipse.Domain.Statistics;

namespace Eclipse.Application.Statistics;

internal static class UserStatisticsExtensions
{
    public static UserStatisticsDto ToDto(this UserStatistics userStatistics)
    {
        return new UserStatisticsDto
        {
            Id = userStatistics.Id,
            UserId = userStatistics.UserId,
            TodoItemsFinished = userStatistics.TodoItemsFinished,
            RemindersReceived = userStatistics.RemindersReceived,
        };
    }
}
