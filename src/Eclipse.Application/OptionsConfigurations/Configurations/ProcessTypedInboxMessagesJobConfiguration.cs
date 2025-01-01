using Eclipse.Application.InboxMessages.Processors.Test;

using Quartz;

namespace Eclipse.Application.OptionsConfigurations.Configurations;

internal sealed class ProcessTypedInboxMessagesJobConfiguration : IJobConfiguration
{
    private static readonly int _delay = 5;

    public void Schedule(QuartzOptions options)
    {
        AddJob<TestEventHandlerProcessorJob>(options);
        AddJob<AnotherTestEventHandlerProcessorJob>(options);
    }

    //private static void AddJob<TEvent, TEventHanlder>(QuartzOptions options)
    //    where TEvent : IDomainEvent
    //    where TEventHanlder : IEventHandler<TEvent>
    //{
    //    var jobKey = JobKey.Create(typeof(ProcessTypedInboxMessagesJob<TEvent, TEventHanlder>).FullName
    //        ?? throw new InvalidOperationException($"Cannot schedule processing for {typeof(TEvent).Name} type"));

    //    options.AddJob<ProcessTypedInboxMessagesJob<TEvent, TEventHanlder>>(job => job.WithIdentity(jobKey))
    //        .AddTrigger(trigger => trigger
    //            .ForJob(jobKey)
    //            .WithSimpleSchedule(schedule => schedule
    //                .WithIntervalInSeconds(_delay)
    //                .RepeatForever())
    //            .StartNow());
    //}

    private static void AddJob<TJob>(QuartzOptions options)
        where TJob : IJob
    {
        var jobKey = JobKey.Create(typeof(TJob).FullName
            ?? throw new InvalidOperationException($"Cannot schedule processing for {typeof(TJob).Name} type"));

        options.AddJob<TJob>(job => job.WithIdentity(jobKey))
            .AddTrigger(trigger => trigger
                .ForJob(jobKey)
                .WithSimpleSchedule(schedule => schedule
                    .WithIntervalInSeconds(_delay)
                    .RepeatForever())
                .StartNow());
    }
}
