using Eclipse.Application.MoodRecords.Report;
using Eclipse.Common.Notifications;
using Eclipse.Domain.Users.Events;
using Eclipse.Tests.Fixtures;

using NSubstitute;

using Xunit;

namespace Eclipse.Application.Tests.MoodRecords.MoodReport;

public sealed class UnscheduleMoodReportHandlerTests : IClassFixture<SchedulerFactoryFixture>
{
    private readonly SchedulerFactoryFixture _schedulerFixture;

    private readonly INotificationScheduler<MoodReportJob, MoodReportSchedulerOptions> _jobScheduler;

    private readonly UnscheduleMoodReportHandler _sut;

    public UnscheduleMoodReportHandlerTests(SchedulerFactoryFixture schedulerFixture)
    {
        _schedulerFixture = schedulerFixture;
        _jobScheduler = Substitute.For<INotificationScheduler<MoodReportJob, MoodReportSchedulerOptions>>();
        _sut = new UnscheduleMoodReportHandler(_schedulerFixture.SchedulerFactory, _jobScheduler);
    }

    [Fact]
    public async Task Handle_WhenUserDisabled_ThenUnschedulesJob()
    {
        var @event = new UserDisabledDomainEvent(Guid.NewGuid());

        await _sut.Handle(@event);

        await _schedulerFixture.SchedulerFactory.Received().GetScheduler();
        await _jobScheduler.Received().Unschedule(
            _schedulerFixture.Scheduler,
            Arg.Is<MoodReportSchedulerOptions>(o => o.UserId == @event.UserId)
        );
    }
}
