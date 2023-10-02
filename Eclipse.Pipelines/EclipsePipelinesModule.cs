using Eclipse.Core.Core;
using Eclipse.Core.Pipelines;
using Eclipse.Pipelines.Hosted;
using Eclipse.Pipelines.Jobs;
using Eclipse.Pipelines.Jobs.Morning;
using Eclipse.Pipelines.Pipelines;
using Eclipse.Pipelines.Pipelines.EdgeCases;
using Eclipse.Pipelines.UpdateHandler;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

using Quartz;

namespace Eclipse.Pipelines;

/// <summary>
/// Takes responsibility for pipelines registration and implementation
/// </summary>
public static class EclipsePipelinesModule
{
    public static IServiceCollection AddPipelinesModule(this IServiceCollection services)
    {
        services
            .Replace(ServiceDescriptor.Transient<INotFoundPipeline, EclipseNotFoundPipeline>())
            .Replace(ServiceDescriptor.Transient<IAccessDeniedPipeline, EclipseAccessDeniedPipeline>())
            .AddTransient<IEclipseUpdateHandler, EclipseUpdateHandler>()
            .AddSingleton<ITelegramUpdateHandler, TelegramUpdateHandler>();

        services.Scan(tss => tss.FromAssemblyOf<EclipsePipelineBase>()
            .AddClasses(c => c.AssignableTo<PipelineBase>())
            .As<PipelineBase>()
            .AsSelf()
            .WithTransientLifetime());

        services.Scan(tss => tss.FromAssemblyOf<EclipseJobBase>()
            .AddClasses()
            .AsImplementedInterfaces()
            .WithTransientLifetime());

        services.AddHostedService<EclipsePipelinesInitializationService>();

        services.Configure<QuartzOptions>(options =>
        {
            var tzi = TimeZoneInfo.FindSystemTimeZoneById("FLE Standard Time");

            var jobKey = JobKey.Create(nameof(MorningJob));

            options.AddJob<MorningJob>(builder => builder.WithIdentity(jobKey))
                .AddTrigger(builder => builder.ForJob(jobKey)
                    .WithSchedule(
                        CronScheduleBuilder
                            .DailyAtHourAndMinute(9, 0)
                            .InTimeZone(tzi))
                    .StartNow());
        });
        
        return services;
    }
}
