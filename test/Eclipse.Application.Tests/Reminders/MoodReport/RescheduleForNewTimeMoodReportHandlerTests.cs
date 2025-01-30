using Eclipse.Application.Reminders.Core;
using Eclipse.Application.Reminders.MoodReport;
using Eclipse.Domain.Users.Events;

using NSubstitute;

using Quartz;

using Xunit;

namespace Eclipse.Application.Tests.Reminders.MoodReport;

public sealed class RescheduleForNewTimeMoodReportHandlerTests
{
    private readonly ISchedulerFactory _schedulerFactory;

    private readonly IJobScheduler<MoodReportJob, MoodReportSchedulerOptions> _jobScheduler;

    private readonly ScheduleNewTimeMoodReportHandler _sut;

    public RescheduleForNewTimeMoodReportHandlerTests()
    {
        _schedulerFactory = Substitute.For<ISchedulerFactory>();
        _jobScheduler = Substitute.For<IJobScheduler<MoodReportJob, MoodReportSchedulerOptions>>();

        _sut = new ScheduleNewTimeMoodReportHandler(_schedulerFactory, _jobScheduler);
    }

    [Fact]
    public async Task Handle_WhenCalled_ThenReschedulesJob()
    {
        var scheduler = Substitute.For<IScheduler>();
        _schedulerFactory.GetScheduler().Returns(scheduler);

        var @event = new GmtChangedDomainEvent(Guid.NewGuid(), new TimeSpan());

        await _sut.Handle(@event);

        await scheduler.Received().DeleteJob(Arg.Is<JobKey>(key => key.Name == $"{nameof(MoodReportJob)}-{@event.UserId}"));

        await _jobScheduler.Received().Schedule(scheduler,
            Arg.Is<MoodReportSchedulerOptions>(o => o.UserId == @event.UserId && o.Gmt == @event.Gmt)
        );
    }
}
