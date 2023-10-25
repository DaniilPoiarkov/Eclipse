using Eclipse.Pipelines.Jobs.Morning;
using Eclipse.Pipelines.Jobs.Reminders;

using Microsoft.Extensions.Options;

using Quartz;

namespace Eclipse.Pipelines.Configurations;

internal class QuatzOptionsConfiguration : IConfigureOptions<QuartzOptions>
{
    private static readonly TimeZoneInfo _timeZone = TimeZoneInfo.FindSystemTimeZoneById("FLE Standard Time");

    private static readonly int _remindersScanInterval = 1;

    public void Configure(QuartzOptions options)
    {
        AddMorningJob(options);
        AddSendRemindersJob(options);
    }

    private static void AddSendRemindersJob(QuartzOptions options)
    {
        var jobKey = JobKey.Create(nameof(SendRemindersJob));

        options.AddJob<SendRemindersJob>(job => job.WithIdentity(jobKey))
            .AddTrigger(trigger => trigger.ForJob(jobKey)
                .WithSimpleSchedule(s => s.WithIntervalInMinutes(_remindersScanInterval)
                    .RepeatForever())
                .StartNow());
    }

    private static void AddMorningJob(QuartzOptions options)
    {
        var key = JobKey.Create(nameof(MorningJob));

        options.AddJob<MorningJob>(job => job.WithIdentity(key))
            .AddTrigger(trigger => trigger.ForJob(key)
                .WithSchedule(
                    CronScheduleBuilder
                        .DailyAtHourAndMinute(9, 0)
                        .InTimeZone(_timeZone))
                .StartNow());
    }
}
