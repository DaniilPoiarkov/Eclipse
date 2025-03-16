using Eclipse.Application.Notifications.GoodMorning;
using Eclipse.Common.Clock;

using Newtonsoft.Json;

using NSubstitute;

using Quartz;

using Xunit;

namespace Eclipse.Application.Tests.Reminders.GoodMorning;

public sealed class GoodMorningSchedulerTests
{
    private readonly ITimeProvider _timeProvider;

    private readonly GoodMorningScheduler _sut;

    public GoodMorningSchedulerTests()
    {
        _timeProvider = Substitute.For<ITimeProvider>();
        _sut = new GoodMorningScheduler(_timeProvider);
    }

    [Fact]
    public async Task Schedule_WhenScheduler_ThenCreateJobWithCorrectIdentityAndData()
    {
        var scheduler = Substitute.For<IScheduler>();
        var options = new GoodMorningSchedulerOptions(Guid.NewGuid(), TimeSpan.FromHours(2));

        var currentTime = DateTime.UtcNow;

        _timeProvider.Now.Returns(currentTime);

        var expectedJobKey = JobKey.Create($"{nameof(GoodMorningJob)}-{options.UserId}");
        var expectedData = JsonConvert.SerializeObject(new GoodMorningJobData(options.UserId));

        await _sut.Schedule(scheduler, options);

        await scheduler.Received().ScheduleJob(
            Arg.Is<IJobDetail>(job =>
                job.Key.Equals(expectedJobKey) &&
                job.JobDataMap.GetString("data") == expectedData
            ),
            Arg.Is<ITrigger>(trigger => trigger.JobKey.Equals(expectedJobKey))
        );
    }
}
