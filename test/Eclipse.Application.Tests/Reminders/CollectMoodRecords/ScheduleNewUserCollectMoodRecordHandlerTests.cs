using Eclipse.Application.MoodRecords.Collection;
using Eclipse.Common.Notifications;
using Eclipse.Domain.Users;
using Eclipse.Domain.Users.Events;
using Eclipse.Tests.Generators;

using Microsoft.Extensions.Logging;

using NSubstitute;

using Quartz;

using Xunit;

namespace Eclipse.Application.Tests.Reminders.CollectMoodRecords;

public sealed class ScheduleNewUserCollectMoodRecordHandlerTests
{
    private readonly IUserRepository _userRepository;

    private readonly ISchedulerFactory _schedulerFactory;

    private readonly INotificationScheduler<CollectMoodRecordJob, CollectMoodRecordSchedulerOptions> _jobScheduler;

    private readonly ILogger<ScheduleNewUserCollectMoodRecordHandler> _logger;

    private readonly ScheduleNewUserCollectMoodRecordHandler _sut;

    public ScheduleNewUserCollectMoodRecordHandlerTests()
    {
        _userRepository = Substitute.For<IUserRepository>();
        _schedulerFactory = Substitute.For<ISchedulerFactory>();
        _jobScheduler = Substitute.For<INotificationScheduler<CollectMoodRecordJob, CollectMoodRecordSchedulerOptions>>();
        _logger = Substitute.For<ILogger<ScheduleNewUserCollectMoodRecordHandler>>();

        _sut = new ScheduleNewUserCollectMoodRecordHandler(_userRepository, _schedulerFactory, _jobScheduler, _logger);
    }

    [Theory]
    [InlineData("test", "test", "test")]
    public async Task Handle_WhenUserNotFound_ThenErrorLogged(string userName, string name, string surname)
    {
        var userId = Guid.NewGuid();

        var @event = new NewUserJoinedDomainEvent(userId, userName, name, surname);

        await _sut.Handle(@event);

        _logger.Log(
            LogLevel.Error,
            Arg.Any<EventId>(),
            Arg.Any<object>(),
            Arg.Any<Exception>(),
            Arg.Any<Func<object, Exception?, string>>()
        );
    }

    [Fact]
    public async Task Handle_WhenUserFound_ThenSchedulesJob()
    {
        var user = UserGenerator.Get();

        var @event = new NewUserJoinedDomainEvent(user.Id, user.UserName, user.Name, user.Surname);

        _userRepository.FindAsync(user.Id).Returns(user);

        var scheduler = Substitute.For<IScheduler>();
        _schedulerFactory.GetScheduler().Returns(scheduler);

        await _sut.Handle(@event);

        await _schedulerFactory.Received().GetScheduler();
        await _jobScheduler.Received().Schedule(scheduler,
            Arg.Is<CollectMoodRecordSchedulerOptions>(o => o.UserId == user.Id && o.Gmt == user.Gmt)
        );
    }
}
