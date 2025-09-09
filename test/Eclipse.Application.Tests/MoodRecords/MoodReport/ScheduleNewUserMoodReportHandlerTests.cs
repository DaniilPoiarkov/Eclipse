using Eclipse.Application.Jobs;
using Eclipse.Application.MoodRecords.Report;
using Eclipse.Application.MoodRecords.Report.Handlers;
using Eclipse.Common.Notifications;
using Eclipse.Domain.Users;
using Eclipse.Domain.Users.Events;
using Eclipse.Tests.Extensions;
using Eclipse.Tests.Fixtures;
using Eclipse.Tests.Generators;

using Microsoft.Extensions.Logging;

using NSubstitute;

using Xunit;

namespace Eclipse.Application.Tests.MoodRecords.MoodReport;

public sealed class ScheduleNewUserMoodReportHandlerTests : IClassFixture<SchedulerFactoryFixture>
{
    private readonly SchedulerFactoryFixture _schedulerFixture;

    private readonly IUserRepository _userRepository;

    private readonly ILogger<ScheduleNewUserMoodReportHandler> _logger;

    private readonly INotificationScheduler<MoodReportJob, SchedulerOptions> _jobScheduler;

    private readonly ScheduleNewUserMoodReportHandler _sut;

    public ScheduleNewUserMoodReportHandlerTests(SchedulerFactoryFixture schedulerFixture)
    {
        _schedulerFixture = schedulerFixture;
        _userRepository = Substitute.For<IUserRepository>();
        _logger = Substitute.For<ILogger<ScheduleNewUserMoodReportHandler>>();
        _jobScheduler = Substitute.For<INotificationScheduler<MoodReportJob, SchedulerOptions>>();

        _sut = new ScheduleNewUserMoodReportHandler(_userRepository, _schedulerFixture.SchedulerFactory, _logger, _jobScheduler);
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
    public async Task Handle_WhenUserJoined_ThenSchedulesJob()
    {
        var user = UserGenerator.Get();

        var @event = new NewUserJoinedDomainEvent(user.Id, user.UserName, user.Name, user.Surname);

        _userRepository.FindAsync(user.Id).Returns(user);

        await _sut.Handle(@event);

        await _schedulerFixture.SchedulerFactory.Received().GetScheduler();
        await _jobScheduler.Received().Schedule(_schedulerFixture.Scheduler,
            Arg.Is<SchedulerOptions>(o => o.UserId == user.Id && o.Gmt == user.Gmt)
        );
    }
}

public sealed class ScheduleUserEnabledMoodReportHandlerTests : IClassFixture<SchedulerFactoryFixture>
{
    private readonly SchedulerFactoryFixture _schedulerFixture;

    private readonly IUserRepository _userRepository;

    private readonly ILogger<ScheduleUserEnabledMoodReportHandler> _logger;

    private readonly INotificationScheduler<MoodReportJob, SchedulerOptions> _jobScheduler;

    private readonly ScheduleUserEnabledMoodReportHandler _sut;

    public ScheduleUserEnabledMoodReportHandlerTests(SchedulerFactoryFixture schedulerFixture)
    {
        _schedulerFixture = schedulerFixture;
        _userRepository = Substitute.For<IUserRepository>();
        _logger = Substitute.For<ILogger<ScheduleUserEnabledMoodReportHandler>>();
        _jobScheduler = Substitute.For<INotificationScheduler<MoodReportJob, SchedulerOptions>>();

        _sut = new ScheduleUserEnabledMoodReportHandler(_userRepository, _schedulerFixture.SchedulerFactory, _logger, _jobScheduler);
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
            Arg.Is<SchedulerOptions>(o => o.UserId == user.Id && o.Gmt == user.Gmt)
        );
    }
}
