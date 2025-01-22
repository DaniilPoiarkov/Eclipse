using Eclipse.Application.Reminders.Core;
using Eclipse.Application.Reminders.Core.Handlers;
using Eclipse.Domain.Users;
using Eclipse.Domain.Users.Events;
using Eclipse.Tests.Generators;

using Microsoft.Extensions.Logging;

using NSubstitute;

using Quartz;

using Xunit;

namespace Eclipse.Application.Tests.Reminders.Core.Handlers;

public sealed class NewUserJoinedEventHandlerTests
{
    private readonly IUserRepository _userRepository;

    private readonly ISchedulerFactory _schedulerFactory;

    private readonly ILogger<NewUserJoinedEventHandler<IJob, object>> _logger;

    private readonly IJobScheduler<IJob, object> _jobScheduler;

    private readonly IOptionsConvertor<User, object> _convertor;

    private readonly NewUserJoinedEventHandler<IJob, object> _sut;

    public NewUserJoinedEventHandlerTests()
    {
        _userRepository = Substitute.For<IUserRepository>();
        _schedulerFactory = Substitute.For<ISchedulerFactory>();
        _logger = Substitute.For<ILogger<NewUserJoinedEventHandler<IJob, object>>>();
        _jobScheduler = Substitute.For<IJobScheduler<IJob, object>>();
        _convertor = Substitute.For<IOptionsConvertor<User, object>>();

        _sut = new NewUserJoinedEventHandler<IJob, object>(_userRepository, _schedulerFactory, _logger, _jobScheduler, _convertor);
    }

    [Fact]
    public async Task Handle_WhenUserNotExists_ThenLogsError()
    {
        _userRepository.FindAsync(Arg.Any<Guid>())
            .Returns(Task.FromResult<User?>(null));

        await _sut.Handle(new NewUserJoinedDomainEvent(Guid.NewGuid(), "", "", ""));

        await _schedulerFactory.DidNotReceive().GetScheduler();
        await _jobScheduler.DidNotReceive().Schedule(Arg.Any<IScheduler>(), Arg.Any<object>());

        _logger.Received().Log(
            LogLevel.Error,
            Arg.Any<EventId>(),
            Arg.Any<object>(),
            Arg.Any<Exception>(),
            Arg.Any<Func<object, Exception?, string>>()
        );
    }

    [Fact]
    public async Task Handle_WhenUserExist_ThenSchedulesJob()
    {
        var user = UserGenerator.Get();

        _userRepository.FindAsync(user.Id).Returns(user);

        var @event = new NewUserJoinedDomainEvent(user.Id, user.UserName, user.Name, user.Surname);

        await _sut.Handle(@event);

        await _schedulerFactory.Received().GetScheduler();
        await _jobScheduler.Received().Schedule(Arg.Any<IScheduler>(), Arg.Any<object>());

        _convertor.Received().Convert(user);
    }
}
