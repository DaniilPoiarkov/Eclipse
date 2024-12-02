using Eclipse.Application.Statistics;
using Eclipse.Domain.Statistics;

using FluentAssertions;

using NSubstitute;

using Xunit;

namespace Eclipse.Application.Tests.Statistics;

public sealed class UserStatisticsServiceTests
{
    private readonly IUserStatisticsRepository _repository;

    private readonly UserStatisticsService _sut;

    public UserStatisticsServiceTests()
    {
        _repository = Substitute.For<IUserStatisticsRepository>();
        _sut = new UserStatisticsService(_repository);
    }

    [Theory]
    [InlineData(10)]
    [InlineData(1)]
    [InlineData(0)]
    public async Task GetByUserId_WhenRequested_ThenProperStatisticsReturned(int todoItemsFinished)
    {
        var statistics = new UserStatistics(Guid.NewGuid(), Guid.NewGuid());

        statistics.ReminderReceived();

        for (int i = 0; i < todoItemsFinished; i++)
        {
            statistics.TodoItemFinished();
        }

        _repository.FindByUserIdAsync(statistics.UserId).Returns(statistics);

        var result = await _sut.GetByUserIdAsync(statistics.UserId);

        result.Id.Should().Be(statistics.Id);
        result.UserId.Should().Be(statistics.UserId);
        result.TodoItemsFinished.Should().Be(todoItemsFinished);
        result.RemindersReceived.Should().Be(1);
    }

    [Fact]
    public async Task GetByUserId_WhenNoStatisticsExists_ThenResultWithDefaultValuesReturned()
    {
        _repository.FindByUserIdAsync(Arg.Any<Guid>()).Returns(Task.FromResult<UserStatistics?>(null));

        var result = await _sut.GetByUserIdAsync(Guid.NewGuid());

        result.Id.Should().BeEmpty();
        result.TodoItemsFinished.Should().Be(0);
        result.RemindersReceived.Should().Be(0);
    }
}
