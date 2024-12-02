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

    [Fact]
    public void ReminderReceived_WhenSpecified_ThenIncrementValue()
    {
        var statistics = new UserStatistics(Guid.NewGuid(), Guid.NewGuid());

        statistics.ReminderReceived();
        statistics.RemindersReceived.Should().Be(1);

        statistics.ReminderReceived();
        statistics.RemindersReceived.Should().Be(2);
    }
}
