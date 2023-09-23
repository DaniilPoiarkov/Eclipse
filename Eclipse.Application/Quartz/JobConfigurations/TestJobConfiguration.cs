using Eclipse.Application.Quartz.Jobs;
using Eclipse.Infrastructure.Quartz;

using Quartz;

namespace Eclipse.Application.Quartz.JobConfigurations;

internal class TestJobConfiguration : IJobConfiguration
{
    private readonly TestJobConfigurationOptions _options;

    public TestJobConfiguration(TestJobConfigurationOptions options)
    {
        _options = options;
    }

    public IJobDetail BuildJob()
    {
        return JobBuilder.Create<TestJob>()
            .UsingJobData("id", _options.ChatId)
            .Build();
    }

    public ITrigger BuildTrigger()
    {
        return TriggerBuilder.Create()
            //.WithSchedule(
            //    CronScheduleBuilder
            //        .DailyAtHourAndMinute(_options.Hours, _options.Minutes)
            //        .InTimeZone(_options.TimeZone))
            .WithSimpleSchedule(s =>
                s.WithIntervalInSeconds(5)
                    .RepeatForever())
            .StartNow()
            .Build();
    }
}
