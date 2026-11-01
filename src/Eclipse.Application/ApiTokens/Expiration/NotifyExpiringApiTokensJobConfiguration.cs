using Eclipse.Application.OptionsConfigurations;
using Eclipse.Common.Notifications;

using Quartz;

namespace Eclipse.Application.ApiTokens.Expiration;

internal sealed class NotifyExpiringApiTokensJobConfiguration : IJobConfiguration
{
    public void Schedule(IQuartzBuilder quartz)
    {
        var jobKey = new JobKey(nameof(NotifyExpiringApiTokensJob), "api-tokens");

        quartz.AddJob<NotifyExpiringApiTokensJob>(job => job.WithIdentity(jobKey))
            .AddTrigger(trigger => trigger.ForJob(jobKey)
                .WithCalendarIntervalSchedule(s =>
                    s.WithInterval(NotificationConsts.OneUnit, IntervalUnit.Day))
                        .StartAt(DateTime.UtcNow.Date.AddDays(NotificationConsts.OneUnit)
                            .WithTime(NotificationConsts.Day130PM)
                        )
            );
    }
}
