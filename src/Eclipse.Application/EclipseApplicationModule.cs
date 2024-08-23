using Eclipse.Application.Account;
using Eclipse.Application.Authorization;
using Eclipse.Application.Configuration;
using Eclipse.Application.Contracts.Account;
using Eclipse.Application.Contracts.Authorization;
using Eclipse.Application.Contracts.Configuration;
using Eclipse.Application.Contracts.Exporting;
using Eclipse.Application.Contracts.Google.Sheets;
using Eclipse.Application.Contracts.Reminders;
using Eclipse.Application.Contracts.Suggestions;
using Eclipse.Application.Contracts.Telegram;
using Eclipse.Application.Contracts.Telegram.Commands;
using Eclipse.Application.Contracts.TodoItems;
using Eclipse.Application.Contracts.Url;
using Eclipse.Application.Contracts.Users;
using Eclipse.Application.Exporting;
using Eclipse.Application.Reminders;
using Eclipse.Application.Suggestions;
using Eclipse.Application.Telegram;
using Eclipse.Application.Telegram.Commands;
using Eclipse.Application.TodoItems;
using Eclipse.Application.Url;
using Eclipse.Application.Users;
using Eclipse.Application.Users.EventHandlers;
using Eclipse.Application.Users.Services;
using Eclipse.Common.Background;

using MediatR.NotificationPublishers;

using Microsoft.Extensions.DependencyInjection;

namespace Eclipse.Application;

/// <summary>
/// Takes responsibility for use cases
/// </summary>
public static class EclipseApplicationModule
{
    public static IServiceCollection AddApplicationModule(this IServiceCollection services)
    {
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
                .AddTransient<IConfigurationService, ConfigurationService>();

        services
            .AddTransient<IUserCreateUpdateService, UserCreateUpdateService>()
            .AddTransient<IUserReadService, UserReadService>()
            .AddTransient<IUserService, UserService>();

        services.Scan(tss => tss.FromAssemblies(typeof(EclipseApplicationModule).Assembly)
            .AddClasses(c => c.AssignableTo(typeof(IEclipseSheetsService<>)))
            .AsImplementedInterfaces()
            .WithTransientLifetime());

        services.Scan(tss => tss.FromAssemblies(typeof(EclipseApplicationModule).Assembly)
            .AddClasses(c => c.AssignableTo(typeof(IBackgroundJob<>)))
            .AsSelf()
            .WithTransientLifetime());

        services.Scan(tss => tss.FromAssemblies(typeof(EclipseApplicationModule).Assembly)
            .AddClasses(c => c.AssignableTo<IImportStrategy>())
            .AsImplementedInterfaces()
            .WithTransientLifetime());

        services.Scan(tss => tss.FromAssemblies(typeof(EclipseApplicationModule).Assembly)
            .AddClasses(c => c.AssignableTo(typeof(IImportValidator<,>)))
            .AsImplementedInterfaces()
            .WithTransientLifetime());

        services.AddMediatR(cfg =>
        {
            cfg.NotificationPublisher = new TaskWhenAllPublisher();
            cfg.RegisterServicesFromAssemblyContaining<NewUserJoinedEventHandler>();
        });

        return services;
    }
}
