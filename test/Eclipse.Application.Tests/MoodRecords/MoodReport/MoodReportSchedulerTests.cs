using Eclipse.Application.MoodRecords.Report;
using Eclipse.Common.Clock;

using Newtonsoft.Json;

using NSubstitute;

using Quartz;

using System.Collections;

using Xunit;

namespace Eclipse.Application.Tests.MoodRecords.MoodReport;

public sealed class MoodReportSchedulerTests
{
    private readonly ITimeProvider _timeProvider;

    private readonly MoodReportScheduler _sut;

    public MoodReportSchedulerTests()
    {
        _timeProvider = Substitute.For<ITimeProvider>();

        _sut = new MoodReportScheduler(_timeProvider);
    }

    [Fact]
    public async Task Schedule_WhenScheduler_ThenCreateJobWithCorrectIdentityAndData()
    {
        var scheduler = Substitute.For<IScheduler>();
        var options = new MoodReportSchedulerOptions(Guid.NewGuid(), TimeSpan.FromHours(2));

        var currentTime = new DateTime(2025, 4, 27, 19, 30, 0);

        _timeProvider.Now.Returns(currentTime);

        var expectedJobKey = JobKey.Create($"{nameof(MoodReportJob)}-{options.UserId}");
        var expectedData = JsonConvert.SerializeObject(new MoodReportJobData(options.UserId));

        await _sut.Schedule(scheduler, options);

        await scheduler.Received().ScheduleJob(
            Arg.Is<IJobDetail>(job =>
                job.Key.Equals(expectedJobKey) &&
                job.JobDataMap.GetString("data") == expectedData
            ),
            Arg.Is<ITrigger>(trigger => trigger.JobKey.Equals(expectedJobKey))
        );
    }

    [Fact]
    public async Task Unschedule_WhenCalled_ThenDeletesJob()
    {
        var scheduler = Substitute.For<IScheduler>();
        var options = new MoodReportSchedulerOptions(Guid.NewGuid(), default);

        await _sut.Unschedule(scheduler, options);
        await scheduler.Received().DeleteJob(Arg.Is<JobKey>(k => k.Name == $"{nameof(MoodReportJob)}-{options.UserId}"));
    }
}
