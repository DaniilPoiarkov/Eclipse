using Eclipse.Application.Feedbacks;
using Eclipse.Application.Feedbacks.Collection;
using Eclipse.Application.InboxMessages;
using Eclipse.Application.Jobs;
using Eclipse.Application.MoodRecords.Collection;
using Eclipse.Application.MoodRecords.Report.Weekly;
using Eclipse.Application.Notifications.FinishTodoItems;
using Eclipse.Application.Notifications.GoodMorning;
using Eclipse.Application.Notifications.NewUserJoined;
using Eclipse.Application.Notifications.Test;
using Eclipse.Application.Reminders;
using Eclipse.Application.Statistics.ReminderReceived;
using Eclipse.Application.Statistics.TodoItemFinished;
using Eclipse.Application.Suggestions;
using Eclipse.Common.Events;
using Eclipse.Domain.Feedbacks;
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
        AddJob<NewUserJoinedDomainEvent, UserEventHandlerBase<NewUserJoinedDomainEvent, FinishTodoItemsJob>>(options);
        AddJob<NewUserJoinedDomainEvent, UserEventHandlerBase<NewUserJoinedDomainEvent, GoodMorningJob>>(options);
        AddJob<NewUserJoinedDomainEvent, UserEventHandlerBase<NewUserJoinedDomainEvent, WeeklyMoodReportJob>>(options);
        AddJob<NewUserJoinedDomainEvent, UserEventHandlerBase<NewUserJoinedDomainEvent, CollectMoodRecordJob>>(options);
        AddJob<NewUserJoinedDomainEvent, UserEventHandlerBase<NewUserJoinedDomainEvent, CollectFeedbackJob>>(options);

        /// <see cref="NewSuggestionSentDomainEvent"/>
        AddJob<NewSuggestionSentDomainEvent, NewSuggestionSentEventHandler>(options);

        /// <see cref="FeedbackSentEvent"/>
        AddJob<FeedbackSentEvent, FeedbackSentEventHandler>(options);

        /// <see cref="ReminderAddedDomainEvent"/>
        AddJob<ReminderAddedDomainEvent, ReminderAddedEventHandler>(options);

        /// <see cref="RemindersReceivedDomainEvent"/>
        AddJob<RemindersReceivedDomainEvent, RemindersReceivedEventHandler>(options);

        /// <see cref="TodoItemFinishedDomainEvent"/>
        AddJob<TodoItemFinishedDomainEvent, TodoItemFinishedEventHandler>(options);

        /// <see cref="GmtChangedDomainEvent"/>
        AddJob<GmtChangedDomainEvent, NewTimeEventHandler<FinishTodoItemsJob>>(options);
        AddJob<GmtChangedDomainEvent, NewTimeEventHandler<GoodMorningJob>>(options);
        AddJob<GmtChangedDomainEvent, NewTimeEventHandler<WeeklyMoodReportJob>>(options);
        AddJob<GmtChangedDomainEvent, NewTimeEventHandler<CollectMoodRecordJob>>(options);
        AddJob<GmtChangedDomainEvent, NewTimeEventHandler<CollectFeedbackJob>>(options);

        /// <see cref="UserDisabledDomainEvent"/>
        AddJob<UserDisabledDomainEvent, UserDisabledEventHandler<CollectMoodRecordJob>>(options);
        AddJob<UserDisabledDomainEvent, UserDisabledEventHandler<WeeklyMoodReportJob>>(options);
        AddJob<UserDisabledDomainEvent, UserDisabledEventHandler<FinishTodoItemsJob>>(options);
        AddJob<UserDisabledDomainEvent, UserDisabledEventHandler<GoodMorningJob>>(options);
        AddJob<UserDisabledDomainEvent, UserDisabledEventHandler<CollectFeedbackJob>>(options);
        AddJob<UserDisabledDomainEvent, UnscheduleAllRemindersHandler>(options);

        /// <see cref="UserEnabledDomainEvent"/>
        AddJob<UserEnabledDomainEvent, UserEventHandlerBase<UserEnabledDomainEvent, FinishTodoItemsJob>>(options);
        AddJob<UserEnabledDomainEvent, UserEventHandlerBase<UserEnabledDomainEvent, GoodMorningJob>>(options);
        AddJob<UserEnabledDomainEvent, UserEventHandlerBase<UserEnabledDomainEvent, WeeklyMoodReportJob>>(options);
        AddJob<UserEnabledDomainEvent, UserEventHandlerBase<UserEnabledDomainEvent, CollectMoodRecordJob>>(options);
        AddJob<UserEnabledDomainEvent, UserEventHandlerBase<UserEnabledDomainEvent, CollectFeedbackJob>>(options);
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
