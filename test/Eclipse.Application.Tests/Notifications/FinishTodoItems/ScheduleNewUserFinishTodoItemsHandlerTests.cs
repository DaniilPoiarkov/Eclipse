using Eclipse.Application.Notifications.FinishTodoItems;
using Eclipse.Common.Notifications;
using Eclipse.Domain.Users;
using Eclipse.Domain.Users.Events;
using Eclipse.Tests.Extensions;
using Eclipse.Tests.Generators;

using Microsoft.Extensions.Logging;

using NSubstitute;

using Quartz;

using Xunit;

namespace Eclipse.Application.Tests.Notifications.FinishTodoItems;

public sealed class ScheduleNewUserFinishTodoItemsHandlerTests
{
    private readonly IUserRepository _userRepository = Substitute.For<IUserRepository>();

    private readonly ISchedulerFactory _schedulerFactory = Substitute.For<ISchedulerFactory>();

    private readonly INotificationScheduler<FinishTodoItemsJob, FinishTodoItemsSchedulerOptions> _jobScheduler =
        Substitute.For<INotificationScheduler<FinishTodoItemsJob, FinishTodoItemsSchedulerOptions>>();

    private readonly ILogger<ScheduleNewUserFinishTodoItemsHandler> _logger =
        Substitute.For<ILogger<ScheduleNewUserFinishTodoItemsHandler>>();

    private readonly ScheduleNewUserFinishTodoItemsHandler _sut;

    public ScheduleNewUserFinishTodoItemsHandlerTests()
    {
        _sut = new ScheduleNewUserFinishTodoItemsHandler(
            _userRepository,
            _schedulerFactory,
            _jobScheduler,
            _logger);
    }

    [Fact]
    public async Task Handle_WhenUserNotFound_ThenLogsErrorAndDoesNothing()
    {
        var userId = Guid.NewGuid();
        _userRepository.FindAsync(userId, Arg.Any<CancellationToken>()).Returns((User?)null);

        await _sut.Handle(new NewUserJoinedDomainEvent(userId, "john.doe", "John", "Doe"));

        _logger.ShouldReceiveLog(LogLevel.Error);

        await _schedulerFactory.DidNotReceive().GetScheduler();
        await _jobScheduler.DidNotReceive().Schedule(Arg.Any<IScheduler>(), Arg.Any<FinishTodoItemsSchedulerOptions>());
    }

    [Fact]
    public async Task Handle_WhenUserFound_ThenSchedulesJob()
    {
        var user = UserGenerator.Get();

        _userRepository.FindAsync(user.Id, Arg.Any<CancellationToken>()).Returns(user);

        var scheduler = Substitute.For<IScheduler>();
        _schedulerFactory.GetScheduler().Returns(scheduler);

        await _sut.Handle(new UserEnabledDomainEvent(user.Id));

        await _jobScheduler.Received().Schedule(scheduler,
            Arg.Is<FinishTodoItemsSchedulerOptions>(opts => opts.UserId == user.Id
                && opts.Gmt == user.Gmt)
        );
    }
}
