using Eclipse.Application.Jobs;
using Eclipse.Application.Notifications.GoodMorning;
using Eclipse.Application.Notifications.GoodMorning.Handlers;
using Eclipse.Common.Notifications;
using Eclipse.Domain.Users.Events;

using NSubstitute;

using Quartz;

using Xunit;

namespace Eclipse.Application.Tests.Notifications.GoodMorning;

public sealed class RescheduleForNewTimeGoodMorningHandlerTests
{
    private readonly ISchedulerFactory _schedulerFactory;

    private readonly INotificationScheduler<GoodMorningJob, SchedulerOptions> _jobScheduler;

    private readonly ScheduleNewTimeGoodMorningHandler _sut;

    public RescheduleForNewTimeGoodMorningHandlerTests()
    {
        _schedulerFactory = Substitute.For<ISchedulerFactory>();
        _jobScheduler = Substitute.For<INotificationScheduler<GoodMorningJob, SchedulerOptions>>();

        _sut = new ScheduleNewTimeGoodMorningHandler(_schedulerFactory, _jobScheduler);
    }

    [Fact]
    public async Task Handle_WhenCalled_ThenReschedulesJob()
    {
        var scheduler = Substitute.For<IScheduler>();
        _schedulerFactory.GetScheduler().Returns(scheduler);

        var @event = new GmtChangedDomainEvent(Guid.NewGuid(), new TimeSpan());

        await _sut.Handle(@event);

        await _jobScheduler.Received().Unschedule(scheduler,
            Arg.Is<SchedulerOptions>(o => o.UserId == @event.UserId && o.Gmt == @event.Gmt)
        );
        await _jobScheduler.Received().Schedule(scheduler,
            Arg.Is<SchedulerOptions>(o => o.UserId == @event.UserId && o.Gmt == @event.Gmt)
        );
    }
}
