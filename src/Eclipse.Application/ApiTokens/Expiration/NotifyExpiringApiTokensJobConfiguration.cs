using Eclipse.Application.OptionsConfigurations;
using Eclipse.Common.Notifications;

using Quartz;

namespace Eclipse.Application.ApiTokens.Expiration;

internal sealed class NotifyExpiringApiTokensJobConfiguration : IJobConfiguration
{
    public void Schedule(QuartzOptions options)
    {
        var jobKey = JobKey.Create(nameof(NotifyExpiringApiTokensJob), "api-tokens");

        options.AddJob<NotifyExpiringApiTokensJob>(job => job.WithIdentity(jobKey))
            .AddTrigger(trigger => trigger.ForJob(jobKey)
                .WithCalendarIntervalSchedule(s =>
                    s.WithIntervalInDays(NotificationConsts.OneUnit))
                        .StartAt(DateTime.UtcNow.Date.AddDays(NotificationConsts.OneUnit)
                            .WithTime(NotificationConsts.Day130PM)
                        )
            );
    }
}
