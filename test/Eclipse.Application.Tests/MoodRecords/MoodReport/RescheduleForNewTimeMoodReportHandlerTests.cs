using Eclipse.Application.MoodRecords.Report;
using Eclipse.Common.Notifications;
using Eclipse.Domain.Users.Events;

using NSubstitute;

using Quartz;

using Xunit;

namespace Eclipse.Application.Tests.MoodRecords.MoodReport;

public sealed class RescheduleForNewTimeMoodReportHandlerTests
{
    private readonly ISchedulerFactory _schedulerFactory;

    private readonly INotificationScheduler<MoodReportJob, MoodReportSchedulerOptions> _jobScheduler;

    private readonly ScheduleNewTimeMoodReportHandler _sut;

    public RescheduleForNewTimeMoodReportHandlerTests()
    {
        _schedulerFactory = Substitute.For<ISchedulerFactory>();
        _jobScheduler = Substitute.For<INotificationScheduler<MoodReportJob, MoodReportSchedulerOptions>>();

        _sut = new ScheduleNewTimeMoodReportHandler(_schedulerFactory, _jobScheduler);
    }

    [Fact]
    public async Task Handle_WhenCalled_ThenReschedulesJob()
    {
        var scheduler = Substitute.For<IScheduler>();
        _schedulerFactory.GetScheduler().Returns(scheduler);

        var @event = new GmtChangedDomainEvent(Guid.NewGuid(), new TimeSpan());

        await _sut.Handle(@event);

        await _jobScheduler.Received().Unschedule(scheduler,
            Arg.Is<MoodReportSchedulerOptions>(o => o.UserId == @event.UserId && o.Gmt == @event.Gmt)
        );
        await _jobScheduler.Received().Schedule(scheduler,
            Arg.Is<MoodReportSchedulerOptions>(o => o.UserId == @event.UserId && o.Gmt == @event.Gmt)
        );
    }
}
