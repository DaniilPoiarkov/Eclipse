﻿using Eclipse.Application.MoodRecords.Collection;
using Eclipse.Common.Clock;

using Newtonsoft.Json;

using NSubstitute;

using Quartz;

using Xunit;

namespace Eclipse.Application.Tests.MoodRecords.CollectMoodRecords;

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
        var options = new CollectMoodRecordSchedulerOptions(Guid.NewGuid(), TimeSpan.FromHours(2));

        var currentTime = DateTime.UtcNow;

        _timeProvider.Now.Returns(currentTime);

        var expectedJobKey = JobKey.Create($"{nameof(CollectMoodRecordJob)}-{options.UserId}");
        var expectedData = JsonConvert.SerializeObject(new CollectMoodRecordJobData(options.UserId));

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
        var options = new CollectMoodRecordSchedulerOptions(Guid.NewGuid(), default);

        await _sut.Schedule(scheduler, options);
        await scheduler.Received().DeleteJob(Arg.Is<JobKey>(k => k.Name == $"{nameof(CollectMoodRecordJob)}-{options.UserId}"));
    }
}
