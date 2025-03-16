using Eclipse.Application.Reminders.CollectMoodRecords;
using Eclipse.Application.Reminders.Core;
using Eclipse.Domain.Users.Events;

using NSubstitute;

using Quartz;

using Xunit;

namespace Eclipse.Application.Tests.Reminders.CollectMoodRecords;

public sealed class ScheduleNewTimeCollectMoodRecordHandlerTests
{
    private readonly ISchedulerFactory _schedulerFactory;

    private readonly IJobScheduler<CollectMoodRecordJob, CollectMoodRecordSchedulerOptions> _jobScheduler;

    private readonly ScheduleNewTimeCollectMoodRecordHandler _sut;

    public ScheduleNewTimeCollectMoodRecordHandlerTests()
    {
        _schedulerFactory = Substitute.For<ISchedulerFactory>();
        _jobScheduler = Substitute.For<IJobScheduler<CollectMoodRecordJob, CollectMoodRecordSchedulerOptions>>();
        _sut = new ScheduleNewTimeCollectMoodRecordHandler(_schedulerFactory, _jobScheduler);
    }

    [Fact]
    public async Task Handle_WhenCalled_ThenReschedulesJob()
    {
        var scheduler = Substitute.For<IScheduler>();
        _schedulerFactory.GetScheduler().Returns(scheduler);

        var @event = new GmtChangedDomainEvent(Guid.NewGuid(), TimeSpan.FromHours(2));

        await _sut.Handle(@event);

        await _jobScheduler.Received().Unschedule(scheduler,
            Arg.Is<CollectMoodRecordSchedulerOptions>(o => o.UserId == @event.UserId && o.Gmt == @event.Gmt)
        );
        await _jobScheduler.Received().Schedule(scheduler,
            Arg.Is<CollectMoodRecordSchedulerOptions>(o => o.UserId == @event.UserId && o.Gmt == @event.Gmt)
        );
    }
}
