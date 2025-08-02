using Eclipse.Application.InboxMessages;
using Eclipse.Application.MoodRecords.Collection.Handlers;
using Eclipse.Application.MoodRecords.Report;
using Eclipse.Application.Notifications.FinishTodoItems.Handlers;
using Eclipse.Application.Notifications.GoodMorning;
using Eclipse.Application.Notifications.NewUserJoined;
using Eclipse.Application.Notifications.Test;
using Eclipse.Application.Reminders;
using Eclipse.Application.Statistics.ReminderReceived;
using Eclipse.Application.Statistics.TodoItemFinished;
using Eclipse.Application.Suggestions;
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

        /// <see cref="UserDisabledDomainEvent"/>
        AddJob<UserDisabledDomainEvent, UnscheduleCollectMoodRecordHandler>(options);
        AddJob<UserDisabledDomainEvent, UnscheduleMoodReportHandler>(options);
        AddJob<UserDisabledDomainEvent, UnscheduleFinishTodoItemsHandler>(options);
        AddJob<UserDisabledDomainEvent, UnscheduleGoodMorningHandler>(options);
        AddJob<UserDisabledDomainEvent, UnscheduleAllRemindersHandler>(options);

        /// <see cref="UserEnabledDomainEvent"/>
        AddJob<UserEnabledDomainEvent, ScheduleUserEnabledFinishTodoItemsHandler>(options);
        AddJob<UserEnabledDomainEvent, ScheduleNewUserGoodMorningHandler>(options);
        AddJob<UserEnabledDomainEvent, ScheduleNewUserMoodReportHandler>(options);
        AddJob<UserEnabledDomainEvent, ScheduleUserEnabledCollectMoodRecordHandler>(options);
        AddJob<UserEnabledDomainEvent, RescheduleRemindersHandler>(options);
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
