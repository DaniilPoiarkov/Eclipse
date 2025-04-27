using Eclipse.Application.Notifications.FinishTodoItems;
using Eclipse.Common.Notifications;
using Eclipse.Domain.Users.Events;
using Eclipse.Tests.Fixtures;

using NSubstitute;

using Xunit;

namespace Eclipse.Application.Tests.Notifications.FinishTodoItems;

public sealed class UnscheduleFinishTodoItemsHandlerTests : IClassFixture<SchedulerFactoryFixture>
{
    private readonly SchedulerFactoryFixture _schedulerFactoryFixture;

    private readonly INotificationScheduler<FinishTodoItemsJob, FinishTodoItemsSchedulerOptions> _jobScheduler;

    private readonly UnscheduleFinishTodoItemsHandler _sut;

    public UnscheduleFinishTodoItemsHandlerTests(SchedulerFactoryFixture schedulerFactoryFixture)
    {
        _schedulerFactoryFixture = schedulerFactoryFixture;
        _jobScheduler = Substitute.For<INotificationScheduler<FinishTodoItemsJob, FinishTodoItemsSchedulerOptions>>();

        _sut = new UnscheduleFinishTodoItemsHandler(_schedulerFactoryFixture.SchedulerFactory, _jobScheduler);
    }

    [Fact]
    public async Task Handle_WhenCalled_ThenUnschedulesJob()
    {
        var @event = new UserDisabledDomainEvent(Guid.NewGuid());

        await _sut.Handle(@event);

        await _jobScheduler.Received().Unschedule(_schedulerFactoryFixture.Scheduler,
            Arg.Is<FinishTodoItemsSchedulerOptions>(o => o.UserId == @event.UserId)
        );
    }
}
