using Eclipse.Application.Notifications.FinishTodoItems;
using Eclipse.Application.Notifications.FinishTodoItems.Handlers;
using Eclipse.Common.Notifications;
using Eclipse.Domain.Users;
using Eclipse.Domain.Users.Events;
using Eclipse.Tests.Generators;

using Microsoft.Extensions.Logging;

using NSubstitute;

using Quartz;

using Xunit;

namespace Eclipse.Application.Tests.Notifications.FinishTodoItems;

public sealed class ScheduleUserEnabledFinishTodoItemsHandlerTests
{
    private readonly IUserRepository _userRepository = Substitute.For<IUserRepository>();

    private readonly ISchedulerFactory _schedulerFactory = Substitute.For<ISchedulerFactory>();

    private readonly INotificationScheduler<FinishTodoItemsJob, FinishTodoItemsSchedulerOptions> _jobScheduler =
        Substitute.For<INotificationScheduler<FinishTodoItemsJob, FinishTodoItemsSchedulerOptions>>();

    private readonly ILogger<ScheduleUserEnabledFinishTodoItemsHandler> _logger =
        Substitute.For<ILogger<ScheduleUserEnabledFinishTodoItemsHandler>>();

    private readonly ScheduleUserEnabledFinishTodoItemsHandler _sut;

    public ScheduleUserEnabledFinishTodoItemsHandlerTests()
    {
        _sut = new ScheduleUserEnabledFinishTodoItemsHandler(
            _userRepository,
            _schedulerFactory,
            _jobScheduler,
            _logger
        );
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
