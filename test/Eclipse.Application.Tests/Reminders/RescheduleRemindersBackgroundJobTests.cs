using Eclipse.Application.Reminders;
using Eclipse.Common.Clock;
using Eclipse.Domain.Users;
using Eclipse.Tests.Generators;

using NSubstitute;

using Quartz;

using System.Linq.Expressions;

using Xunit;

namespace Eclipse.Application.Tests.Reminders;

public sealed class RescheduleRemindersBackgroundJobTests
{
    private readonly IUserRepository _userRepository;

    private readonly ISchedulerFactory _schedulerFactory;

    private readonly ITimeProvider _timeProvider;

    private readonly RescheduleRemindersBackgroundJob _sut;

    public RescheduleRemindersBackgroundJobTests()
    {
        _userRepository = Substitute.For<IUserRepository>();
        _schedulerFactory = Substitute.For<ISchedulerFactory>();
        _timeProvider = Substitute.For<ITimeProvider>();

        _sut = new RescheduleRemindersBackgroundJob(_userRepository, _schedulerFactory, _timeProvider);
    }

    [Theory]
    [InlineData("Test", 12, 0)]
    public async Task ExecuteAsync_WhenCalled_ShouldRescheduleReminders(string text, int hour, int minute)
    {
        var user = UserGenerator.Get();

        user.AddReminder(text, new TimeOnly(hour, minute));
        user.AddReminder(text, new TimeOnly(hour, minute));

        _userRepository.GetByExpressionAsync(Arg.Any<Expression<Func<User, bool>>>()).Returns([user]);

        var scheduler = Substitute.For<IScheduler>();

        _schedulerFactory.GetScheduler().Returns(scheduler);

        await _sut.Execute();

        await scheduler.Received(2).ScheduleJob(Arg.Any<IJobDetail>(), Arg.Any<ITrigger>());
    }
}
