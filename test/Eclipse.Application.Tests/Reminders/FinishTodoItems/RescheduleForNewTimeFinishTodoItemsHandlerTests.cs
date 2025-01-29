using Eclipse.Application.Reminders.Core;
using Eclipse.Application.Reminders.FinishTodoItems;
using Eclipse.Domain.Users.Events;

using NSubstitute;

using Quartz;

using Xunit;

namespace Eclipse.Application.Tests.Reminders.FinishTodoItems;

public sealed class RescheduleForNewTimeFinishTodoItemsHandlerTests
{
    private readonly ISchedulerFactory _schedulerFactory;

    private readonly IJobScheduler<FinishTodoItemsJob, FinishTodoItemsSchedulerOptions> _jobScheduler;

    private readonly ScheduleNewTimeFinishTodoItemsHandler _sut;

    public RescheduleForNewTimeFinishTodoItemsHandlerTests()
    {
        _schedulerFactory = Substitute.For<ISchedulerFactory>();
        _jobScheduler = Substitute.For<IJobScheduler<FinishTodoItemsJob, FinishTodoItemsSchedulerOptions>>();

        _sut = new ScheduleNewTimeFinishTodoItemsHandler(_schedulerFactory, _jobScheduler);
    }

    [Fact]
    public async Task Handle_WhenCalled_ThenReschedulesJob()
    {
        var scheduler = Substitute.For<IScheduler>();
        _schedulerFactory.GetScheduler().Returns(scheduler);

        var @event = new GmtChangedDomainEvent(Guid.NewGuid(), new TimeSpan());

        await _sut.Handle(@event);

        await scheduler.Received().DeleteJob(Arg.Is<JobKey>(key => key.Name == $"{nameof(FinishTodoItemsJob)}-{@event.UserId}"));

        await _jobScheduler.Received().Schedule(scheduler,
            Arg.Is<FinishTodoItemsSchedulerOptions>(o => o.UserId == @event.UserId && o.Gmt == @event.Gmt)
        );
    }
}
