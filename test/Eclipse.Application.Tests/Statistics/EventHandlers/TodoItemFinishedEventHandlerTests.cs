using Eclipse.Application.Statistics.EventHandlers;
using Eclipse.Domain.Statistics;
using Eclipse.Domain.Users.Events;

using FluentAssertions;

using NSubstitute;

using Xunit;

namespace Eclipse.Application.Tests.Statistics.EventHandlers;

public sealed class TodoItemFinishedEventHandlerTests
{
    private readonly IUserStatisticsRepository _repository;

    private readonly TodoItemFinishedEventHandler _sut;

    public TodoItemFinishedEventHandlerTests()
    {
        _repository = Substitute.For<IUserStatisticsRepository>();
        _sut = new TodoItemFinishedEventHandler(_repository);
    }

    [Fact]
    public async Task Handle_WhenStatisticsExists_ThenIncrementValue()
    {
        var statistics = new UserStatistics(Guid.NewGuid(), Guid.NewGuid());

        _repository.FindByUserIdAsync(statistics.UserId, CancellationToken.None).Returns(statistics);

        var notification = new TodoItemFinishedDomainEvent(statistics.UserId);

        await _sut.Handle(notification, CancellationToken.None);

        statistics.TodoItemsFinished.Should().Be(1);
        await _repository.Received().FindByUserIdAsync(statistics.UserId);
        await _repository.DidNotReceiveWithAnyArgs().CreateAsync(Arg.Any<UserStatistics>());
        await _repository.Received().UpdateAsync(statistics);
    }

    [Fact]
    public async Task Handle_WhenStatisticsNotExists_ThenCreateAndProcess()
    {
        var statistics = new UserStatistics(Guid.NewGuid(), Guid.NewGuid());
        _repository.CreateAsync(Arg.Any<UserStatistics>(), Arg.Any<CancellationToken>()).Returns(statistics);

        var notification = new TodoItemFinishedDomainEvent(statistics.UserId);
        await _sut.Handle(notification, Arg.Any<CancellationToken>());

        await _repository.Received().CreateAsync(Arg.Is<UserStatistics>(us => us.UserId == statistics.UserId));
        await _repository.Received().UpdateAsync(statistics);
    }
}
