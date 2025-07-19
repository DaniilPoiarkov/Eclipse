using Eclipse.Application.MoodRecords.Collection;
using Eclipse.Common.Notifications;
using Eclipse.Domain.Users.Events;
using Eclipse.Tests.Fixtures;

using NSubstitute;

using Xunit;

namespace Eclipse.Application.Tests.MoodRecords.CollectMoodRecords;

public sealed class UnscheduleCollectMoodRecordHandlerTests : IClassFixture<SchedulerFactoryFixture>
{
    private readonly SchedulerFactoryFixture _schedulerFixture;

    private readonly INotificationScheduler<CollectMoodRecordJob, CollectMoodRecordSchedulerOptions> _jobScheduler;

    private readonly UnscheduleCollectMoodRecordHandler _sut;

    public UnscheduleCollectMoodRecordHandlerTests(SchedulerFactoryFixture scheduler)
    {
        _schedulerFixture = scheduler;
        _jobScheduler = Substitute.For<INotificationScheduler<CollectMoodRecordJob, CollectMoodRecordSchedulerOptions>>();

        _sut = new UnscheduleCollectMoodRecordHandler(scheduler.SchedulerFactory, _jobScheduler);
    }

    [Fact]
    public async Task Handle_WhenUserDisabled_ThenUnschedulesJob()
    {
        var @event = new UserDisabledDomainEvent(Guid.NewGuid());

        await _sut.Handle(@event);

        await _schedulerFixture.SchedulerFactory.Received().GetScheduler();
        await _jobScheduler.Received().Unschedule(
            _schedulerFixture.Scheduler,
            Arg.Is<CollectMoodRecordSchedulerOptions>(o => o.UserId == @event.UserId)
        );
    }
}
