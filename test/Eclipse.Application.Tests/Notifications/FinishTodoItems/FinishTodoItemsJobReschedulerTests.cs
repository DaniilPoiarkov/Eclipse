using Eclipse.Application.Notifications.FinishTodoItems;
using Eclipse.Common.Notifications;
using Eclipse.Domain.Users;
using Eclipse.Tests.Fixtures;
using Eclipse.Tests.Generators;

using NSubstitute;

using System.Linq.Expressions;

using Xunit;

namespace Eclipse.Application.Tests.Notifications.FinishTodoItems;

public sealed class FinishTodoItemsJobReschedulerTests : IClassFixture<SchedulerFactoryFixture>
{
    private readonly IUserRepository _userRepository;

    private readonly INotificationScheduler<FinishTodoItemsJob, FinishTodoItemsSchedulerOptions> _notificationScheduler;

    private readonly SchedulerFactoryFixture _schedulerFactoryFixture;

    private readonly FinishTodoItemsJobRescheduler _sut;

    public FinishTodoItemsJobReschedulerTests(SchedulerFactoryFixture schedulerFactoryFixture)
    {
        _userRepository = Substitute.For<IUserRepository>();
        _notificationScheduler = Substitute.For<INotificationScheduler<FinishTodoItemsJob, FinishTodoItemsSchedulerOptions>>();
        _schedulerFactoryFixture = schedulerFactoryFixture;

        _sut = new FinishTodoItemsJobRescheduler(_userRepository, schedulerFactoryFixture.SchedulerFactory, _notificationScheduler);
    }

    [Fact]
    public async Task Execute_WhenCalled_ThenRescheduleJobs()
    {
        var users = UserGenerator.Generate(5);

        _userRepository.GetByExpressionAsync(Arg.Any<Expression<Func<User, bool>>>())
            .Returns(users);

        await _sut.Execute();

        await _notificationScheduler.Received(users.Count)
            .Schedule(
                _schedulerFactoryFixture.Scheduler,
                Arg.Is<FinishTodoItemsSchedulerOptions>(o => users.Exists(u => u.Id == o.UserId))
            );
    }
}
