using Eclipse.Application.Reminders.Core;
using Eclipse.Application.Reminders.GoodMorning;
using Eclipse.Domain.Users.Events;

using NSubstitute;

using Quartz;

using Xunit;

namespace Eclipse.Application.Tests.Reminders.GoodMorning;

public sealed class RescheduleForNewTimeGoodMorningHandlerTests
{
    private readonly ISchedulerFactory _schedulerFactory;

    private readonly IJobScheduler<RegularJob<GoodMorningJob, GoodMorningJobData>, GoodMorningSchedulerOptions> _jobScheduler;

    private readonly ScheduleNewTimeGoodMorningHandler _sut;

    public RescheduleForNewTimeGoodMorningHandlerTests()
    {
        _schedulerFactory = Substitute.For<ISchedulerFactory>();
        _jobScheduler = Substitute.For<IJobScheduler<RegularJob<GoodMorningJob, GoodMorningJobData>, GoodMorningSchedulerOptions>>();

        _sut = new ScheduleNewTimeGoodMorningHandler(_schedulerFactory, _jobScheduler);
    }

    [Fact]
    public async Task Handle_WhenCalled_ThenReschedulesJob()
    {
        var scheduler = Substitute.For<IScheduler>();
        _schedulerFactory.GetScheduler().Returns(scheduler);

        var @event = new GmtChangedDomainEvent(Guid.NewGuid(), new TimeSpan());

        await _sut.Handle(@event);

        await scheduler.Received().DeleteJob(Arg.Is<JobKey>(key => key.Name == $"{nameof(GoodMorningJob)}-{@event.UserId}"));

        await _jobScheduler.Received().Schedule(scheduler,
            Arg.Is<GoodMorningSchedulerOptions>(o => o.UserId == @event.UserId && o.Gmt == @event.Gmt)
        );
    }
}
