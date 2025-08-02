using Eclipse.Application.MoodRecords.Collection;
using Eclipse.Application.MoodRecords.Collection.Handlers;
using Eclipse.Common.Notifications;
using Eclipse.Domain.Users;
using Eclipse.Domain.Users.Events;
using Eclipse.Tests.Extensions;
using Eclipse.Tests.Fixtures;
using Eclipse.Tests.Generators;

using Microsoft.Extensions.Logging;

using NSubstitute;

using Xunit;

namespace Eclipse.Application.Tests.MoodRecords.CollectMoodRecords;

public sealed class ScheduleUserEnabledCollectMoodRecordHandlerTests : IClassFixture<SchedulerFactoryFixture>
{
    private readonly SchedulerFactoryFixture _schedulerFixture;

    private readonly IUserRepository _userRepository;

    private readonly INotificationScheduler<CollectMoodRecordJob, CollectMoodRecordSchedulerOptions> _jobScheduler;

    private readonly ILogger<ScheduleUserEnabledCollectMoodRecordHandler> _logger;

    private readonly ScheduleUserEnabledCollectMoodRecordHandler _sut;

    public ScheduleUserEnabledCollectMoodRecordHandlerTests(SchedulerFactoryFixture schedulerFixture)
    {
        _schedulerFixture = schedulerFixture;
        _userRepository = Substitute.For<IUserRepository>();
        _jobScheduler = Substitute.For<INotificationScheduler<CollectMoodRecordJob, CollectMoodRecordSchedulerOptions>>();
        _logger = Substitute.For<ILogger<ScheduleUserEnabledCollectMoodRecordHandler>>();

        _sut = new ScheduleUserEnabledCollectMoodRecordHandler(_userRepository, _schedulerFixture.SchedulerFactory, _jobScheduler, _logger);
    }

    [Fact]
    public async Task Handle_WhenUserEnabled_ThenSchedulesJob()
    {
        var user = UserGenerator.Get();

        var @event = new UserEnabledDomainEvent(user.Id);

        _userRepository.FindAsync(user.Id).Returns(user);

        await _sut.Handle(@event);

        await _schedulerFixture.SchedulerFactory.Received().GetScheduler();
        await _jobScheduler.Received().Schedule(_schedulerFixture.Scheduler,
            Arg.Is<CollectMoodRecordSchedulerOptions>(o => o.UserId == user.Id && o.Gmt == user.Gmt)
        );
    }

    [Fact]
    public async Task Handle_WhenEnabledUserNotFound_ThenErrorLogged()
    {
        var @event = new UserEnabledDomainEvent(Guid.NewGuid());

        await _sut.Handle(@event);

        _logger.ShouldReceiveLog(LogLevel.Error);
    }
}
