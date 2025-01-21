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
using Eclipse.Application.OptionsConfigurations;
using Eclipse.Application.OutboxMessages;
using Eclipse.Application.Reminders;
using Eclipse.Application.Reminders.Core;
using Eclipse.Application.Reminders.FinishTodoItems;
using Eclipse.Application.Reminders.GoodMorning;
using Eclipse.Application.Reminders.MoodReport;
using Eclipse.Application.Reports;
using Eclipse.Application.Statistics;
using Eclipse.Application.Suggestions;
using Eclipse.Application.Telegram;
using Eclipse.Application.Telegram.Commands;
using Eclipse.Application.TodoItems;
using Eclipse.Application.Url;
using Eclipse.Application.Users;
using Eclipse.Application.Users.Services;
using Eclipse.Common.Background;
using Eclipse.Common.Events;

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
            .AddClasses(c => c.AssignableTo(typeof(IJobScheduler<,>)), publicOnly: false)
            .AsImplementedInterfaces()
            .WithSingletonLifetime());

        services.Scan(tss => tss.FromAssemblies(typeof(EclipseApplicationModule).Assembly)
            .AddClasses(c => c.AssignableTo(typeof(IOptionsConvertor<,>)), publicOnly: false)
            .AsImplementedInterfaces()
            .WithSingletonLifetime());

        services.Scan(tss => tss.FromAssemblies(typeof(EclipseApplicationModule).Assembly)
            .AddClasses(c => c.AssignableTo(typeof(INotificationJob<>)), publicOnly: false)
            .AsImplementedInterfaces()
            .WithSingletonLifetime());

        services.ConfigureOptions<QuartzOptionsConfiguration>();

        return services;
    }

    public static async Task InitializeApplicationLayerAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateAsyncScope();

        var manager = scope.ServiceProvider.GetRequiredService<IBackgroundJobManager>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<RescheduleRemindersBackgroundJob>>();

        await manager.EnqueueAsync<Rescheduler<RegularJob<FinishTodoItemsJob, FinishTodoItemsJobData>, FinishTodoItemsSchedulerOptions>>();
        logger.LogInformation("Enqueued {Job} job.", nameof(Rescheduler<RegularJob<FinishTodoItemsJob, FinishTodoItemsJobData>, FinishTodoItemsSchedulerOptions>));

        await manager.EnqueueAsync<Rescheduler<RegularJob<GoodMorningJob, GoodMorningJobData>, GoodMorningSchedulerOptions>>();
        logger.LogInformation("Enqueued {Job} job.", nameof(Rescheduler<RegularJob<GoodMorningJob, GoodMorningJobData>, GoodMorningSchedulerOptions>));

        await manager.EnqueueAsync<Rescheduler<RegularJob<MoodReportJob, MoodReportJobData>, MoodReportSchedulerOptions>>();
        logger.LogInformation("Enqueued {Job} job.", nameof(Rescheduler<RegularJob<MoodReportJob, MoodReportJobData>, MoodReportSchedulerOptions>));
    }
}
