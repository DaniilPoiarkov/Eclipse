using Eclipse.Application.InboxMessages;
using Eclipse.Application.Reminders;
using Eclipse.Application.Reminders.CollectMoodRecords;
using Eclipse.Application.Reminders.FinishTodoItems;
using Eclipse.Application.Reminders.GoodMorning;
using Eclipse.Application.Reminders.MoodReport;
using Eclipse.Application.Statistics.EventHandlers;
using Eclipse.Application.Suggestions;
using Eclipse.Application.Users.EventHandlers;
using Eclipse.Application.Users.TestEvents;
using Eclipse.Common.Events;
using Eclipse.Domain.Suggestions;
using Eclipse.Domain.Users.Events;

using Quartz;

namespace Eclipse.Application.OptionsConfigurations.Configurations;

internal sealed class ProcessTypedInboxMessagesJobConfiguration : IJobConfiguration
{
    private static readonly int _delay = 5;

    public void Schedule(QuartzOptions options)
    {
        /// <see cref="TestDomainEvent"/>
        AddJob<TestDomainEvent, TestEventHandler>(options);
        AddJob<TestDomainEvent, AnotherTestEventHandler>(options);

        /// <see cref="NewUserJoinedDomainEvent"/>
        AddJob<NewUserJoinedDomainEvent, NewUserJoinedEventHandler>(options);
        AddJob<NewUserJoinedDomainEvent, ScheduleNewUserFinishTodoItemsHandler>(options);
        AddJob<NewUserJoinedDomainEvent, ScheduleNewUserGoodMorningHandler>(options);
        AddJob<NewUserJoinedDomainEvent, ScheduleNewUserMoodReportHandler>(options);
        AddJob<NewUserJoinedDomainEvent, ScheduleNewUserCollectMoodRecordHandler>(options);

        /// <see cref="NewSuggestionSentDomainEvent"/>
        AddJob<NewSuggestionSentDomainEvent, NewSuggestionSentEventHandler>(options);

        /// <see cref="ReminderAddedDomainEvent"/>
        AddJob<ReminderAddedDomainEvent, ReminderAddedEventHandler>(options);

        /// <see cref="RemindersReceivedDomainEvent"/>
        AddJob<RemindersReceivedDomainEvent, RemindersReceivedEventHandler>(options);

        /// <see cref="TodoItemFinishedDomainEvent"/>
        AddJob<TodoItemFinishedDomainEvent, TodoItemFinishedEventHandler>(options);

        /// <see cref="GmtChangedDomainEvent"/>
        AddJob<GmtChangedDomainEvent, ScheduleNewTimeFinishTodoItemsHandler>(options);
        AddJob<GmtChangedDomainEvent, ScheduleNewTimeGoodMorningHandler>(options);
        AddJob<GmtChangedDomainEvent, ScheduleNewTimeMoodReportHandler>(options);
        AddJob<GmtChangedDomainEvent, ScheduleNewTimeCollectMoodRecordHandler>(options);
    }

    private static void AddJob<TEvent, TEventHanlder>(QuartzOptions options)
        where TEvent : IDomainEvent
        where TEventHanlder : IEventHandler<TEvent>
    {
        var jobKey = JobKey.Create(typeof(ProcessTypedInboxMessagesJob<TEvent, TEventHanlder>).FullName
            ?? throw new InvalidOperationException($"Cannot schedule processing for {typeof(TEvent).Name} type"));

        options.AddJob<ProcessTypedInboxMessagesJob<TEvent, TEventHanlder>>(job => job.WithIdentity(jobKey))
            .AddTrigger(trigger => trigger
                .ForJob(jobKey)
                .WithSimpleSchedule(schedule => schedule
                    .WithIntervalInSeconds(_delay)
                    .RepeatForever())
                .StartNow());
    }
}
