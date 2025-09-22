using Eclipse.Application.Jobs;
using Eclipse.Application.Notifications.GoodMorning;
using Eclipse.Common.Notifications;
using Eclipse.Domain.Users;
using Eclipse.Tests.Fixtures;
using Eclipse.Tests.Generators;

using NSubstitute;

using Quartz;

using System.Linq.Expressions;

using Xunit;

namespace Eclipse.Application.Tests.Notifications.GoodMorning;

public sealed class GoodMorningJobReschedulerTests : IClassFixture<SchedulerFactoryFixture>
{
    private readonly IUserRepository _userRepository;

    private readonly ISchedulerFactory _schedulerFactory;

    private readonly INotificationScheduler<GoodMorningJob, SchedulerOptions> _jobScheduler;

    private readonly SchedulerFactoryFixture _schedulerFactoryFixture;

    private readonly GoodMorningJobRescheduler _sut;

    public GoodMorningJobReschedulerTests(SchedulerFactoryFixture schedulerFactoryFixture)
    {
        _userRepository = Substitute.For<IUserRepository>();
        _schedulerFactory = schedulerFactoryFixture.SchedulerFactory;
        _jobScheduler = Substitute.For<INotificationScheduler<GoodMorningJob, SchedulerOptions>>();
        _schedulerFactoryFixture = schedulerFactoryFixture;

        _sut = new GoodMorningJobRescheduler(_userRepository, _schedulerFactory, _jobScheduler);
    }

    [Fact]
    public async Task Execute_WhenCalled_ThenReschedulesJob()
    {
        var users = UserGenerator.Generate(5);

        _userRepository.GetByExpressionAsync(Arg.Any<Expression<Func<User, bool>>>())
            .Returns(users);

        await _sut.Execute();

        await _jobScheduler.Received(users.Count).Schedule(
            _schedulerFactoryFixture.Scheduler,
            Arg.Is<SchedulerOptions>(o => users.Exists(u => u.Id == o.UserId))
        );
    }
}
