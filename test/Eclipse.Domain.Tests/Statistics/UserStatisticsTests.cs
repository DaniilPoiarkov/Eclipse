using Eclipse.Domain.Statistics;

using FluentAssertions;

using Xunit;

namespace Eclipse.Domain.Tests.Statistics;

public sealed class UserStatisticsTests
{
    [Fact]
    public void TodoItemFinished_WhenSpecified_ThenIncrementValue()
    {
        var statistics = new UserStatistics(Guid.NewGuid(), Guid.NewGuid());

        statistics.TodoItemFinished();
        statistics.TodoItemsFinished.Should().Be(1);

        statistics.TodoItemFinished();
        statistics.TodoItemsFinished.Should().Be(2);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(5)]
    public void ReminderReceived_WhenSpecified_ThenIncrementValue(int count)
    {
        var statistics = new UserStatistics(Guid.NewGuid(), Guid.NewGuid());

        statistics.ReminderReceived(count);
        statistics.RemindersReceived.Should().Be(count);

        statistics.ReminderReceived(count);
        statistics.RemindersReceived.Should().Be(count * 2);
    }
}
