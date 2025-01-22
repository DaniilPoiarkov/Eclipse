using Eclipse.Application.Reminders.Core;
using Eclipse.Domain.Users;
using Eclipse.Tests.Generators;

using NSubstitute;

using Quartz;

using Xunit;

namespace Eclipse.Application.Tests.Reminders.Core;

public sealed class ReschedulerTests
{
    private readonly IUserRepository _userRepository;

    private readonly ISchedulerFactory _schedulerFactory;

    private readonly IJobScheduler<IJob, object> _jobScheduler;

    private readonly IOptionsConvertor<User, object> _convertor;

    private readonly Rescheduler<IJob, object> _sut;

    public ReschedulerTests()
    {
        _userRepository = Substitute.For<IUserRepository>();
        _schedulerFactory = Substitute.For<ISchedulerFactory>();
        _jobScheduler = Substitute.For<IJobScheduler<IJob, object>>();
        _convertor = Substitute.For<IOptionsConvertor<User, object>>();

        _sut = new Rescheduler<IJob, object>(_userRepository, _schedulerFactory, _jobScheduler, _convertor);
    }

    [Fact]
    public async Task ExecuteAsync_WhenTriggered_ThenReschedulesJobs()
    {
        var users = UserGenerator.Generate(5);

        var scheduler = Substitute.For<IScheduler>();

        _userRepository.GetAllAsync().Returns(users);
        _schedulerFactory.GetScheduler().Returns(scheduler);

        await _sut.ExecuteAsync();

        await _userRepository.Received().GetAllAsync();
        await _schedulerFactory.Received().GetScheduler();
        await _jobScheduler.Received(users.Count).Schedule(scheduler, Arg.Any<object>());

        _convertor.Received(users.Count).Convert(
            Arg.Is<User>(u => users.Contains(u))
        );
    }
}
