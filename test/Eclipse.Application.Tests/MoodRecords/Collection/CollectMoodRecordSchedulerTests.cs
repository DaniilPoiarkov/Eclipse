using Eclipse.Application.Jobs;
using Eclipse.Application.MoodRecords.Collection;
using Eclipse.Common.Clock;

using Newtonsoft.Json;

using NSubstitute;

using Quartz;

using Xunit;

namespace Eclipse.Application.Tests.MoodRecords.Collection;

public sealed class CollectMoodRecordSchedulerTests
{
    private readonly ITimeProvider _timeProvider;

    private readonly CollectMoodRecordScheduler _sut;

    public CollectMoodRecordSchedulerTests()
    {
        _timeProvider = Substitute.For<ITimeProvider>();
        _sut = new CollectMoodRecordScheduler(_timeProvider);
    }

    [Fact]
    public async Task Schedule_WhenCalled_ThenSchedulesJob()
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
        await scheduler.Received().UnscheduleJob(Arg.Is<TriggerKey>(k => k.Name == $"{nameof(CollectMoodRecordJob)}-{options.UserId}"));
    }
}
