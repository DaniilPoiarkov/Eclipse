using Eclipse.Application.Account;
using Eclipse.Application.Authorization;
using Eclipse.Application.Configuration;
using Eclipse.Application.Contracts.Account;
using Eclipse.Application.Contracts.Authorization;
using Eclipse.Application.Contracts.Configuration;
using Eclipse.Application.Contracts.Exporting;
using Eclipse.Application.Contracts.Google.Sheets;
using Eclipse.Application.Contracts.InboxMessages;
using Eclipse.Application.Contracts.MoodRecords;
using Eclipse.Application.Contracts.OutboxMessages;
using Eclipse.Application.Contracts.Reminders;
using Eclipse.Application.Contracts.Reports;
using Eclipse.Application.Contracts.Statistics;
using Eclipse.Application.Contracts.Suggestions;
using Eclipse.Application.Contracts.Telegram;
using Eclipse.Application.Contracts.Telegram.Commands;
using Eclipse.Application.Contracts.TodoItems;
using Eclipse.Application.Contracts.Url;
using Eclipse.Application.Contracts.Users;
using Eclipse.Application.Exporting;
using Eclipse.Application.InboxMessages;
using Eclipse.Application.MoodRecords;
using Eclipse.Application.MoodRecords.Collection;
using Eclipse.Application.MoodRecords.Report;
using Eclipse.Application.Notifications.FinishTodoItems;
using Eclipse.Application.Notifications.GoodMorning;
using Eclipse.Application.OptionsConfigurations;
using Eclipse.Application.OutboxMessages;
using Eclipse.Application.Reminders;
using Eclipse.Application.Reports;
using Eclipse.Application.Statistics;
using Eclipse.Application.Suggestions;
using Eclipse.Application.Telegram;
using Eclipse.Application.Telegram.Commands;
using Eclipse.Application.TodoItems;
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
                .AddTransient<IConfigurationService, ConfigurationService>()
                .AddTransient<IOutboxMessagesService, OutboxMessagesService>()
                .AddTransient<IInboxMessageService, InboxMessageService>()
                .AddTransient<IInboxMessageConvertor, InboxMessageConvertor>()
                .AddTransient<IReportsService, ReportsService>()
                .AddTransient<IUserStatisticsService, UserStatisticsService>()
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

    public static async Task InitializeApplicationLayerAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateAsyncScope();

        var manager = scope.ServiceProvider.GetRequiredService<IBackgroundJobManager>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<IBackgroundJobManager>>();

        await manager.EnqueueAsync<RescheduleRemindersBackgroundJob>();
        logger.LogInformation("Enqueued {Job} job.", nameof(RescheduleRemindersBackgroundJob));

        await manager.EnqueueAsync<FinishTodoItemsJobRescheduler>();
        logger.LogInformation("Enqueued {Job} job.", nameof(FinishTodoItemsJobRescheduler));

        await manager.EnqueueAsync<GoodMorningJobRescheduler>();
        logger.LogInformation("Enqueued {Job} job.", nameof(GoodMorningJobRescheduler));

        await manager.EnqueueAsync<MoodReportJobRescheduler>();
        logger.LogInformation("Enqueued {Job} job.", nameof(MoodReportJobRescheduler));

        await manager.EnqueueAsync<CollectMoodRecordJobRescheduler>();
        logger.LogInformation("Enqueued {Job} job.", nameof(CollectMoodRecordJobRescheduler));
    }
}
