using Eclipse.Application.Account;
using Eclipse.Application.Authorization;
using Eclipse.Application.Configuration;
using Eclipse.Application.Contracts.Account;
using Eclipse.Application.Contracts.Authorization;
using Eclipse.Application.Contracts.Configuration;
using Eclipse.Application.Contracts.Exporting;
using Eclipse.Application.Contracts.Feedbacks;
using Eclipse.Application.Contracts.Google.Sheets;
using Eclipse.Application.Contracts.InboxMessages;
using Eclipse.Application.Contracts.MoodRecords;
using Eclipse.Application.Contracts.OutboxMessages;
using Eclipse.Application.Contracts.Reminders;
using Eclipse.Application.Contracts.Statistics;
using Eclipse.Application.Contracts.Suggestions;
using Eclipse.Application.Contracts.Telegram;
using Eclipse.Application.Contracts.TodoItems;
using Eclipse.Application.Contracts.Url;
using Eclipse.Application.Contracts.Users;
using Eclipse.Application.Exporting;
using Eclipse.Application.Feedbacks;
using Eclipse.Application.Feedbacks.Collection;
using Eclipse.Application.InboxMessages;
using Eclipse.Application.Jobs;
using Eclipse.Application.MoodRecords;
using Eclipse.Application.MoodRecords.Collection;
using Eclipse.Application.MoodRecords.Report;
using Eclipse.Application.MoodRecords.Report.Monthly;
using Eclipse.Application.MoodRecords.Report.Weekly;
using Eclipse.Application.Notifications.GoodMorning;
using Eclipse.Application.OptionsConfigurations;
using Eclipse.Application.OptionsConfigurations.Registrators.Events;
using Eclipse.Application.OutboxMessages;
using Eclipse.Application.Reminders;
using Eclipse.Application.Statistics;
using Eclipse.Application.Suggestions;
using Eclipse.Application.Telegram;
using Eclipse.Application.TodoItems;
using Eclipse.Application.TodoItems.Finish;
using Eclipse.Application.Url;
using Eclipse.Application.Users;
using Eclipse.Common.Background;
using Eclipse.Common.Events;
using Eclipse.Common.Notifications;

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using Quartz;

namespace Eclipse.Application;

/// <summary>
/// Takes responsibility for use cases
/// </summary>
public static class EclipseApplicationModule
{
    public static IServiceCollection AddApplicationModule(this IServiceCollection services, Action<ApplicationOptions> options)
    {
        services.Configure(options);

        services
            .AddSingleton<IAppUrlProvider, AppUrlProvider>()
                .AddTransient<ICommandService, CommandService>()
                .AddTransient<ISuggestionsService, SuggestionsService>()
                .AddTransient<ITodoItemService, TodoItemService>()
                .AddTransient<ITelegramService, TelegramService>()
                .AddTransient<IReminderService, ReminderService>()
                .AddTransient<IExportService, ExportService>()
                .AddTransient<IImportService, ImportService>()
                .AddTransient<IAccountService, AccountService>()
                .AddTransient<ILoginManager, LoginManager>()
                .AddTransient<IMoodRecordsService, MoodRecordsService>()
                .AddTransient<IMoodReportSender, MoodReportSender>()
                .AddTransient<IConfigurationService, ConfigurationService>()
                .AddTransient<IOutboxMessagesService, OutboxMessagesService>()
                .AddTransient<IInboxMessageService, InboxMessageService>()
                .AddTransient<IInboxMessageConvertor, InboxMessageConvertor>()
                .AddTransient<IMoodReportService, MoodReportService>()
                .AddTransient<IUserStatisticsService, UserStatisticsService>()
                .AddTransient<IFeedbackService, FeedbackService>()
            .AddScoped(typeof(IInboxMessageProcessor<,>), typeof(TypedInboxMessageProcessor<,>));

        services
            .AddTransient<IUserCreateUpdateService, UserCreateUpdateService>()
            .AddTransient<IUserReadService, UserReadService>()
            .AddTransient<IUserService, UserService>();

        services.Scan(tss => tss.FromAssemblies(typeof(EclipseApplicationModule).Assembly)
            .AddClasses(c => c.AssignableTo(typeof(IEclipseSheetsService<>)), publicOnly: false)
            .AsImplementedInterfaces()
            .WithTransientLifetime());

        services.Scan(tss => tss.FromAssemblies(typeof(EclipseApplicationModule).Assembly)
            .AddClasses(c => c.AssignableTo(typeof(IBackgroundJob<>)), publicOnly: false)
            .AsSelf()
            .WithTransientLifetime());

        services.Scan(tss => tss.FromAssemblies(typeof(EclipseApplicationModule).Assembly)
            .AddClasses(c => c.AssignableTo<IBackgroundJob>(), publicOnly: false)
            .AsSelf()
            .WithTransientLifetime());

        services.Scan(tss => tss.FromAssemblies(typeof(EclipseApplicationModule).Assembly)
            .AddClasses(c => c.AssignableTo<IImportStrategy>(), publicOnly: false)
            .AsImplementedInterfaces()
            .WithTransientLifetime());

        services.Scan(tss => tss.FromAssemblies(typeof(EclipseApplicationModule).Assembly)
            .AddClasses(c => c.AssignableTo(typeof(IImportValidator<,>)), publicOnly: false)
            .AsImplementedInterfaces()
            .WithTransientLifetime());

        services.Scan(tss => tss.FromAssemblies(typeof(EclipseApplicationModule).Assembly)
            .AddClasses(c => c.AssignableTo<IJob>(), publicOnly: false)
            .AsSelfWithInterfaces()
            .WithScopedLifetime());

        List<IApplicationServicesRegistrator> registrators = [
            new NewUserJoinedEventRegistrator(),
            new GmtChangedEventRegistrator(),
            new UserDisabledEventRegistrator(),
            new UserEnabledEventRegistrator()
        ];

        foreach (var registrator in registrators)
        {
            registrator.Register(services);
        }

        services.Scan(tss => tss.FromAssemblies(typeof(EclipseApplicationModule).Assembly)
            .AddClasses(c => c.AssignableTo(typeof(IEventHandler<>)), publicOnly: false)
            .AsSelfWithInterfaces()
            .WithTransientLifetime());

        services.Scan(tss => tss.FromAssemblies(typeof(EclipseApplicationModule).Assembly)
            .AddClasses(c => c.AssignableTo(typeof(INotificationScheduler<,>)), publicOnly: false)
            .AsImplementedInterfaces()
            .WithSingletonLifetime());

        services.ConfigureOptions<QuartzOptionsConfiguration>();

        return services;
    }

    public static async Task InitializeApplicationLayerAsync(this WebApplication app, CancellationToken cancellationToken = default)
    {
        await using var scope = app.Services.CreateAsyncScope();

        var manager = scope.ServiceProvider.GetRequiredService<IBackgroundJobManager>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<IBackgroundJobManager>>();

        await manager.EnqueueAsync<RescheduleRemindersBackgroundJob>(cancellationToken);
        logger.LogInformation("Enqueued rescheduling for {Job} job.", nameof(RescheduleRemindersBackgroundJob));

        await manager.EnqueueAsync<JobRescheduler<GoodMorningJob>>(cancellationToken);
        logger.LogInformation("Enqueued rescheduling for {Job} job.", nameof(GoodMorningJob));

        await manager.EnqueueAsync<JobRescheduler<FinishTodoItemsJob>>(cancellationToken);
        logger.LogInformation("Enqueued rescheduling for {Job} job.", nameof(FinishTodoItemsJob));

        await manager.EnqueueAsync<JobRescheduler<CollectMoodRecordJob>>(cancellationToken);
        logger.LogInformation("Enqueued rescheduling for {Job} job.", nameof(CollectMoodRecordJob));

        await manager.EnqueueAsync<JobRescheduler<WeeklyMoodReportJob>>(cancellationToken);
        logger.LogInformation("Enqueued rescheduling for {Job} job.", nameof(WeeklyMoodReportJob));

        await manager.EnqueueAsync<JobRescheduler<MonthlyMoodReportJob>>(cancellationToken);
        logger.LogInformation("Enqueued rescheduling for {Job} job.", nameof(MonthlyMoodReportJob));

        await manager.EnqueueAsync<JobRescheduler<CollectFeedbackJob>>(cancellationToken);
        logger.LogInformation("Enqueued rescheduling for {Job} job.", nameof(CollectFeedbackJob));
    }
}
