using Eclipse.Application.Jobs;
using Eclipse.Application.Notifications.FinishTodoItems;
using Eclipse.Application.Notifications.FinishTodoItems.Handlers;
using Eclipse.Common.Notifications;
using Eclipse.Domain.Users;
using Eclipse.Domain.Users.Events;
using Eclipse.Tests.Extensions;

using Microsoft.Extensions.Logging;

using NSubstitute;

using Quartz;

using Xunit;

namespace Eclipse.Application.Tests.Notifications.FinishTodoItems;

public sealed class ScheduleNewUserFinishTodoItemsHandlerTests
{
    private readonly IUserRepository _userRepository = Substitute.For<IUserRepository>();

    private readonly ISchedulerFactory _schedulerFactory = Substitute.For<ISchedulerFactory>();

    private readonly INotificationScheduler<FinishTodoItemsJob, SchedulerOptions> _jobScheduler =
        Substitute.For<INotificationScheduler<FinishTodoItemsJob, SchedulerOptions>>();

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
        await _jobScheduler.DidNotReceive().Schedule(Arg.Any<IScheduler>(), Arg.Any<SchedulerOptions>());
    }
}
