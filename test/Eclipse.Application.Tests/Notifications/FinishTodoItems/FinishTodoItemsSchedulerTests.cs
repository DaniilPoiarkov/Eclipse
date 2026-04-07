using Eclipse.Application.Jobs;
using Eclipse.Application.TodoItems.Finish;
using Eclipse.Common.Clock;

using Newtonsoft.Json;

using NSubstitute;

using Quartz;

using Xunit;

namespace Eclipse.Application.Tests.Notifications.FinishTodoItems;

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
        var options = new SchedulerOptions(Guid.NewGuid(), TimeSpan.FromHours(2));

        var jobDetail = Substitute.For<IJobDetail>();
        jobDetail.Key.Returns(new JobKey("Test"));

        scheduler.GetJobDetail(Arg.Any<JobKey>()).Returns(jobDetail);

        _timeProvider.Now.Returns(DateTime.UtcNow);

        var expectedData = JsonConvert.SerializeObject(new UserIdJobData(options.UserId));

        await _sut.Schedule(scheduler, options);

        await scheduler.Received().ScheduleJob(
            Arg.Is<IJobDetail>(job => job.Key.Equals(jobDetail.Key)),
            Arg.Is<ITrigger>(trigger => trigger.JobKey.Equals(jobDetail.Key)
                && trigger.JobDataMap.GetString("data") == expectedData
            )
        );
    }

    [Fact]
    public async Task Unschedule_WhenCalled_ThenDeletesJob()
    {
        var scheduler = Substitute.For<IScheduler>();
        var options = new SchedulerOptions(Guid.NewGuid(), default);

        await _sut.Unschedule(scheduler, options);
        await scheduler.Received().UnscheduleJob(Arg.Is<TriggerKey>(k => k.Name == $"{nameof(FinishTodoItemsJob)}-{options.UserId}"));
    }
}
