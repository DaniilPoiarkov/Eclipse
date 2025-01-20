using Eclipse.Application.Reminders.FinishTodoItems;
using Eclipse.Common.Clock;

using Newtonsoft.Json;

using NSubstitute;

using Quartz;

using Xunit;

namespace Eclipse.Application.Tests.Reminders.FinishTodoItems;

public sealed class FinishTodoItemsSchedulerTests
{
    private readonly ITimeProvider _timeProvider;

    private readonly FinishTodoItemsScheduler _sut;

    public FinishTodoItemsSchedulerTests()
    {
        _timeProvider = Substitute.For<ITimeProvider>();

        _sut = new FinishTodoItemsScheduler(_timeProvider);
    }

    [Fact]
    public async Task Schedule_WhenScheduler_ThenCreateJobWithCorrectIdentityAndData()
    {
        var scheduler = Substitute.For<IScheduler>();
        var options = new FinishTodoItemsSchedulerOptions(Guid.NewGuid(), TimeSpan.FromHours(2));

        var currentTime = DateTime.UtcNow;

        _timeProvider.Now.Returns(currentTime);

        var expectedJobKey = JobKey.Create($"{nameof(RemindToFinishTodoItemsJob)}-{options.UserId}");
        var expectedData = JsonConvert.SerializeObject(new RemindToFinishTodoItemsJobData(options.UserId));

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
