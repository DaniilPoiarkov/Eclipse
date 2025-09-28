using Eclipse.Application.Jobs;
using Eclipse.Application.MoodRecords.Report;
using Eclipse.Common.Notifications;
using Eclipse.Domain.Users;
using Eclipse.Tests.Fixtures;
using Eclipse.Tests.Generators;

using NSubstitute;

using System.Linq.Expressions;

using Xunit;

namespace Eclipse.Application.Tests.MoodRecords.MoodReport;

public sealed class MoodReportJobReschedulerTests : IClassFixture<SchedulerFactoryFixture>
{
    private readonly SchedulerFactoryFixture _schedulerFixture;

    private readonly IUserRepository _userRepository;

    private readonly INotificationScheduler<MoodReportJob, SchedulerOptions> _jobScheduler;

    private readonly MoodReportJobRescheduler _sut;

    public MoodReportJobReschedulerTests(SchedulerFactoryFixture schedulerFixture)
    {
        _schedulerFixture = schedulerFixture;
        _userRepository = Substitute.For<IUserRepository>();
        _jobScheduler = Substitute.For<INotificationScheduler<MoodReportJob, SchedulerOptions>>();

        _sut = new MoodReportJobRescheduler(_userRepository, _schedulerFixture.SchedulerFactory, _jobScheduler);
    }

    [Fact]
    public async Task Execute_WhenTriggered_ThenReschedulesJob()
    {
        var users = UserGenerator.Generate(5);

        _userRepository.GetByExpressionAsync(Arg.Any<Expression<Func<User, bool>>>()).Returns(users);

        await _sut.Execute();
        await _jobScheduler.Received(users.Count).Schedule(
            _schedulerFixture.Scheduler,
            Arg.Is<SchedulerOptions>(o => users.Exists(u => u.Id == o.UserId))
        );
    }
}
