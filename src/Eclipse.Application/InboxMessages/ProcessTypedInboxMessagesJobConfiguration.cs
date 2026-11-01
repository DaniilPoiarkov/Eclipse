using Eclipse.Application.Feedbacks;
using Eclipse.Application.Feedbacks.Collection;
using Eclipse.Application.Jobs;
using Eclipse.Application.MoodRecords.Collection;
using Eclipse.Application.MoodRecords.Report.Monthly;
using Eclipse.Application.MoodRecords.Report.Weekly;
using Eclipse.Application.Notifications.GoodMorning;
using Eclipse.Application.OptionsConfigurations;
using Eclipse.Application.Promotions;
using Eclipse.Application.Reminders;
using Eclipse.Application.Statistics.ReminderReceived;
using Eclipse.Application.Statistics.TodoItemFinished;
using Eclipse.Application.Suggestions;
using Eclipse.Application.TodoItems.Finish;
using Eclipse.Application.Users.NewUserJoined;
using Eclipse.Application.Users.Test;
using Eclipse.Common.Events;
using Eclipse.Domain.Feedbacks;
using Eclipse.Domain.Promotions;
using Eclipse.Domain.Suggestions;
using Eclipse.Domain.Users.Events;

using Quartz;

namespace Eclipse.Application.InboxMessages;

internal sealed class ProcessTypedInboxMessagesJobConfiguration : IJobConfiguration
{
    private static readonly int _delay = 5;

    public void Schedule(IQuartzBuilder quartz)
    {
        /// <see cref="TestDomainEvent"/>
        AddJob<TestDomainEvent, TestEventHandler>(quartz);
        AddJob<TestDomainEvent, AnotherTestEventHandler>(quartz);

        /// <see cref="NewUserJoinedDomainEvent"/>
        AddJob<NewUserJoinedDomainEvent, NewUserJoinedEventHandler>(quartz);
        AddJob<NewUserJoinedDomainEvent, UserEventHandlerBase<NewUserJoinedDomainEvent, FinishTodoItemsJob>>(quartz);
        AddJob<NewUserJoinedDomainEvent, UserEventHandlerBase<NewUserJoinedDomainEvent, GoodMorningJob>>(quartz);
        AddJob<NewUserJoinedDomainEvent, UserEventHandlerBase<NewUserJoinedDomainEvent, WeeklyMoodReportJob>>(quartz);
        AddJob<NewUserJoinedDomainEvent, UserEventHandlerBase<NewUserJoinedDomainEvent, MonthlyMoodReportJob>>(quartz);
        AddJob<NewUserJoinedDomainEvent, UserEventHandlerBase<NewUserJoinedDomainEvent, CollectMoodRecordJob>>(quartz);
        AddJob<NewUserJoinedDomainEvent, UserEventHandlerBase<NewUserJoinedDomainEvent, CollectFeedbackJob>>(quartz);

        /// <see cref="NewSuggestionSentDomainEvent"/>
        AddJob<NewSuggestionSentDomainEvent, NewSuggestionSentEventHandler>(quartz);

        /// <see cref="FeedbackSentEvent"/>
        AddJob<FeedbackSentEvent, FeedbackSentEventHandler>(quartz);

        /// <see cref="ReminderAddedDomainEvent"/>
        AddJob<ReminderAddedDomainEvent, ReminderAddedEventHandler>(quartz);

        /// <see cref="RemindersReceivedDomainEvent"/>
        AddJob<RemindersReceivedDomainEvent, RemindersReceivedEventHandler>(quartz);

        /// <see cref="TodoItemFinishedDomainEvent"/>
        AddJob<TodoItemFinishedDomainEvent, TodoItemFinishedEventHandler>(quartz);

        /// <see cref="GmtChangedDomainEvent"/>
        AddJob<GmtChangedDomainEvent, NewTimeEventHandler<FinishTodoItemsJob>>(quartz);
        AddJob<GmtChangedDomainEvent, NewTimeEventHandler<GoodMorningJob>>(quartz);
        AddJob<GmtChangedDomainEvent, NewTimeEventHandler<WeeklyMoodReportJob>>(quartz);
        AddJob<GmtChangedDomainEvent, NewTimeEventHandler<MonthlyMoodReportJob>>(quartz);
        AddJob<GmtChangedDomainEvent, NewTimeEventHandler<CollectMoodRecordJob>>(quartz);
        AddJob<GmtChangedDomainEvent, NewTimeEventHandler<CollectFeedbackJob>>(quartz);

        /// <see cref="UserDisabledDomainEvent"/>
        AddJob<UserDisabledDomainEvent, UserDisabledEventHandler<CollectMoodRecordJob>>(quartz);
        AddJob<UserDisabledDomainEvent, UserDisabledEventHandler<WeeklyMoodReportJob>>(quartz);
        AddJob<UserDisabledDomainEvent, UserDisabledEventHandler<MonthlyMoodReportJob>>(quartz);
        AddJob<UserDisabledDomainEvent, UserDisabledEventHandler<FinishTodoItemsJob>>(quartz);
        AddJob<UserDisabledDomainEvent, UserDisabledEventHandler<GoodMorningJob>>(quartz);
        AddJob<UserDisabledDomainEvent, UserDisabledEventHandler<CollectFeedbackJob>>(quartz);
        AddJob<UserDisabledDomainEvent, UnscheduleAllRemindersHandler>(quartz);

        /// <see cref="UserEnabledDomainEvent"/>
        AddJob<UserEnabledDomainEvent, UserEventHandlerBase<UserEnabledDomainEvent, FinishTodoItemsJob>>(quartz);
        AddJob<UserEnabledDomainEvent, UserEventHandlerBase<UserEnabledDomainEvent, GoodMorningJob>>(quartz);
        AddJob<UserEnabledDomainEvent, UserEventHandlerBase<UserEnabledDomainEvent, WeeklyMoodReportJob>>(quartz);
        AddJob<UserEnabledDomainEvent, UserEventHandlerBase<UserEnabledDomainEvent, MonthlyMoodReportJob>>(quartz);
        AddJob<UserEnabledDomainEvent, UserEventHandlerBase<UserEnabledDomainEvent, CollectMoodRecordJob>>(quartz);
        AddJob<UserEnabledDomainEvent, UserEventHandlerBase<UserEnabledDomainEvent, CollectFeedbackJob>>(quartz);
        AddJob<UserEnabledDomainEvent, RescheduleRemindersHandler>(quartz);

        /// <see cref="ReminderRescheduledDomainEvent"/>
        AddJob<ReminderRescheduledDomainEvent, ReminderRescheduledEventHandler>(quartz);

        /// <see cref="ReminderRemovedDomainEvent"/>
        AddJob<ReminderRemovedDomainEvent, ReminderRemovedEventHandler>(quartz);

        /// <see cref="PromotionPublishingRequestedDomainEvent"/>
        AddJob<PromotionPublishingRequestedDomainEvent, PromotionPublishingRequestedEventHandler>(quartz);
    }

    private static void AddJob<TEvent, TEventHanlder>(IQuartzBuilder quartz)
        where TEvent : IDomainEvent
        where TEventHanlder : IEventHandler<TEvent>
    {
        var jobKey = new JobKey(typeof(ProcessTypedInboxMessagesJob<TEvent, TEventHanlder>).FullName
            ?? throw new InvalidOperationException($"Cannot schedule processing for {typeof(TEvent).Name} type"),
            "events-processing"
        );

        quartz.AddJob<ProcessTypedInboxMessagesJob<TEvent, TEventHanlder>>(job => job.WithIdentity(jobKey))
            .AddTrigger(trigger => trigger
                .ForJob(jobKey)
                .WithSimpleSchedule(schedule => schedule
                    .WithInterval(TimeSpan.FromSeconds(_delay))
                    .RepeatForever())
                .StartNow());
    }
}
