using Eclipse.Application.Contracts.Exporting;
using Eclipse.Application.Contracts.Google.Sheets;
using Eclipse.Application.Contracts.Localizations;
using Eclipse.Application.Contracts.Reminders;
using Eclipse.Application.Contracts.Suggestions;
using Eclipse.Application.Contracts.Telegram;
using Eclipse.Application.Contracts.Telegram.Commands;
using Eclipse.Application.Contracts.TodoItems;
using Eclipse.Application.Contracts.Url;
using Eclipse.Application.Contracts.Users;
using Eclipse.Application.Exporting;
using Eclipse.Application.Google.Sheets;
using Eclipse.Application.Localizations;
using Eclipse.Application.Reminders;
using Eclipse.Application.Suggestions;
using Eclipse.Application.Telegram;
using Eclipse.Application.Telegram.Commands;
using Eclipse.Application.TodoItems;
using Eclipse.Application.Url;
using Eclipse.Application.Users;
using Eclipse.Application.Users.EventHandlers;
using Eclipse.Application.Users.Services;

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
            .AddSingleton<IUserCache, UserCache>()
            .AddSingleton<IAppUrlProvider, AppUrlProvider>()
                .AddTransient<ICommandService, CommandService>()
                .AddTransient<ISuggestionsService, SuggestionsService>()
                .AddTransient<ITodoItemService, TodoItemService>()
                .AddTransient<ITelegramService, TelegramService>()
                .AddTransient<IReminderService, ReminderService>()
                .AddTransient<IExportService, ExportService>()
                .AddTransient<IImportService, ImportService>()
            .AddScoped<IEclipseLocalizer, EclipseLocalizer>();

        services
            .AddTransient<IUserCreateUpdateService, UserCreateUpdateService>()
            .AddTransient<IUserLogicService, UserLogicService>()
            .AddTransient<IUserReadService, UserReadService>()
            .AddTransient<IUserService, UserService>();

        services.Scan(tss => tss.FromAssemblyOf<SuggestionsSheetsService>()
            .AddClasses(c => c.AssignableTo(typeof(IEclipseSheetsService<>)))
            .AsImplementedInterfaces()
            .WithTransientLifetime());

        services.AddMediatR(cfg =>
        {
            cfg.NotificationPublisher = new TaskWhenAllPublisher();
            cfg.RegisterServicesFromAssemblyContaining<NewUserJoinedEventHandler>();
        });

        services
            .Decorate<IReminderService, CachedReminderService>()
            .Decorate<IUserService, CachedUserService>()
            .Decorate<ITodoItemService, CachedTodoItemsService>();

        return services;
    }
}
