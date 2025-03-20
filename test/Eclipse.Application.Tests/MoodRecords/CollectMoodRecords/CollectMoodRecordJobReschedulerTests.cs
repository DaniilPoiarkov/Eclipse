using Eclipse.Application.MoodRecords.Collection;
using Eclipse.Common.Notifications;
using Eclipse.Domain.Users;
using Eclipse.Tests.Generators;

using NSubstitute;

using Quartz;

using System.Linq.Expressions;

using Xunit;

namespace Eclipse.Application.Tests.MoodRecords.CollectMoodRecords;

public sealed class CollectMoodRecordJobReschedulerTests
{
    private readonly IUserRepository _userRepository;

    private readonly ISchedulerFactory _schedulerFactory;

    private readonly INotificationScheduler<CollectMoodRecordJob, CollectMoodRecordSchedulerOptions> _jobScheduler;

    private readonly CollectMoodRecordJobRescheduler _sut;

    public CollectMoodRecordJobReschedulerTests()
    {
        _userRepository = Substitute.For<IUserRepository>();
        _schedulerFactory = Substitute.For<ISchedulerFactory>();
        _jobScheduler = Substitute.For<INotificationScheduler<CollectMoodRecordJob, CollectMoodRecordSchedulerOptions>>();

        _sut = new CollectMoodRecordJobRescheduler(_userRepository, _schedulerFactory, _jobScheduler);
    }

    [Fact]
    public async Task Execute_WhenCalled_ThenScheduleJobForEachUser()
    {
        var users = UserGenerator.Generate(5);

        _userRepository.GetByExpressionAsync(Arg.Any<Expression<Func<User, bool>>>()).Returns(users);

        var scheduler = Substitute.For<IScheduler>();
        _schedulerFactory.GetScheduler().Returns(scheduler);

        await _sut.ExecuteAsync();

        await _jobScheduler.Received(users.Count).Schedule(scheduler,
            Arg.Is<CollectMoodRecordSchedulerOptions>(o =>
                users.Exists(u => u.Id == o.UserId && u.Gmt == o.Gmt)
            )
        );
    }
}
