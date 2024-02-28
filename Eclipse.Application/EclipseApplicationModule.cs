using Eclipse.Application.Contracts.Google.Sheets;
using Eclipse.Application.Contracts.IdentityUsers;
using Eclipse.Application.Contracts.Localizations;
using Eclipse.Application.Contracts.Reminders;
using Eclipse.Application.Contracts.Suggestions;
using Eclipse.Application.Contracts.Telegram;
using Eclipse.Application.Contracts.Telegram.Commands;
using Eclipse.Application.Contracts.TodoItems;
using Eclipse.Application.Google.Sheets.Suggestions;
using Eclipse.Application.IdentityUsers;
using Eclipse.Application.IdentityUsers.EventHandlers;
using Eclipse.Application.IdentityUsers.Services;
using Eclipse.Application.Localizations;
using Eclipse.Application.Reminders;
using Eclipse.Application.Suggestions;
using Eclipse.Application.Telegram;
using Eclipse.Application.Telegram.Commands;
using Eclipse.Application.TodoItems;

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
            .AddSingleton<IIdentityUserCache, IdentityUserCache>()
                .AddTransient<ICommandService, CommandService>()
                .AddTransient<ISuggestionsService, SuggestionsService>()
                .AddTransient<ITodoItemService, TodoItemService>()
                .AddTransient<ITelegramService, TelegramService>()
                .AddTransient<IReminderService, ReminderService>()
            .AddScoped<IEclipseLocalizer, EclipseLocalizer>();

        services
            .AddTransient<IIdentityUserCreateUpdateService, IdentityUserCreateUpdateService>()
            .AddTransient<IIdentityUserLogicService, IdentityUserLogicService>()
            .AddTransient<IIdentityUserReadService, IdentityUserReadService>()
            .AddTransient<IIdentityUserService, IdentityUserService>();

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
            .Decorate<IIdentityUserService, CachedIdentityUserService>()
            .Decorate<ITodoItemService, CachedTodoItemsService>();

        return services;
    }
}
