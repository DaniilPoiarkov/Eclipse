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

public sealed class ScheduleNewUserCollectMoodRecordHandlerTests : IClassFixture<SchedulerFactoryFixture>
{
    private readonly SchedulerFactoryFixture _schedulerFixture;

    private readonly IUserRepository _userRepository;

    private readonly INotificationScheduler<CollectMoodRecordJob, CollectMoodRecordSchedulerOptions> _jobScheduler;

    private readonly ILogger<ScheduleNewUserCollectMoodRecordHandler> _logger;

    private readonly ScheduleNewUserCollectMoodRecordHandler _sut;

    public ScheduleNewUserCollectMoodRecordHandlerTests(SchedulerFactoryFixture schedulerFixture)
    {
        _schedulerFixture = schedulerFixture;
        _userRepository = Substitute.For<IUserRepository>();
        _jobScheduler = Substitute.For<INotificationScheduler<CollectMoodRecordJob, CollectMoodRecordSchedulerOptions>>();
        _logger = Substitute.For<ILogger<ScheduleNewUserCollectMoodRecordHandler>>();

        _sut = new ScheduleNewUserCollectMoodRecordHandler(_userRepository, _schedulerFixture.SchedulerFactory, _jobScheduler, _logger);
    }

    [Theory]
    [InlineData("test", "test", "test")]
    public async Task Handle_WhenJoinedUserNotFound_ThenErrorLogged(string userName, string name, string surname)
    {
        var @event = new NewUserJoinedDomainEvent(Guid.NewGuid(), userName, name, surname);

        await _sut.Handle(@event);

        _logger.ShouldReceiveLog(LogLevel.Error);
    }

    [Fact]
    public async Task Handle_WhenEnabledUserNotFound_ThenErrorLogged()
    {
        var @event = new UserEnabledDomainEvent(Guid.NewGuid());

        await _sut.Handle(@event);

        _logger.ShouldReceiveLog(LogLevel.Error);
    }

    [Fact]
    public async Task Handle_WhenUserJoined_ThenSchedulesJob()
    {
        var user = UserGenerator.Get();

        var @event = new NewUserJoinedDomainEvent(user.Id, user.UserName, user.Name, user.Surname);

        _userRepository.FindAsync(user.Id).Returns(user);

        await _sut.Handle(@event);

        await _schedulerFixture.SchedulerFactory.Received().GetScheduler();
        await _jobScheduler.Received().Schedule(_schedulerFixture.Scheduler,
            Arg.Is<CollectMoodRecordSchedulerOptions>(o => o.UserId == user.Id && o.Gmt == user.Gmt)
        );
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
}
