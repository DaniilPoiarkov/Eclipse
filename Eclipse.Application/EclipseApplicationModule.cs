using Eclipse.Application.Contracts.Base;
using Eclipse.Application.Contracts.Google.Sheets.Suggestions;
using Eclipse.Application.Contracts.Google.Sheets.TodoItems;
using Eclipse.Application.Contracts.Google.Sheets.Users;
using Eclipse.Application.Contracts.IdentityUsers;
using Eclipse.Application.Contracts.Localizations;
using Eclipse.Application.Contracts.Reminders;
using Eclipse.Application.Contracts.Suggestions;
using Eclipse.Application.Contracts.Telegram;
using Eclipse.Application.Contracts.Telegram.Commands;
using Eclipse.Application.Contracts.Telegram.Messages;
using Eclipse.Application.Contracts.Telegram.Pipelines;
using Eclipse.Application.Contracts.TodoItems;
using Eclipse.Application.Google.Sheets.Parsers;
using Eclipse.Application.Google.Sheets.Suggestions;
using Eclipse.Application.Google.Sheets.TodoItems;
using Eclipse.Application.Google.Sheets.Users;
using Eclipse.Application.Hosted;
using Eclipse.Application.IdentityUsers;
using Eclipse.Application.Localizations;
using Eclipse.Application.Reminders;
using Eclipse.Application.Suggestions;
using Eclipse.Application.Telegram;
using Eclipse.Application.Telegram.Commands;
using Eclipse.Application.Telegram.Messages;
using Eclipse.Application.Telegram.Pipelines;
using Eclipse.Application.TodoItems;
using Eclipse.Infrastructure.Google.Sheets;

using FluentValidation;

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
            .AddSingleton<IPipelineStore, PipelineStore>()
            .AddSingleton<IMessageStore, MessageStore>()
                .AddTransient<ICommandService, CommandService>()
                .AddTransient<ISuggestionsService, SuggestionsService>()
                .AddTransient<ITodoItemService, TodoItemService>()
                .AddTransient<ITelegramService, TelegramService>()
                .AddTransient<IEclipseLocalizer, EclipseLocalizer>()
                .AddTransient<IIdentityUserService, IdentityUserService>()
                .AddTransient<IIdentityUserStore, IdentityUserStore>()
                .AddTransient<IReminderService, ReminderService>();

        services.AddValidatorsFromAssemblyContaining<CommandDtoValidator>(ServiceLifetime.Transient);

        services.Scan(tss => tss.FromAssemblyOf<SuggestionObjectParser>()
            .AddClasses(c => c.AssignableTo(typeof(IObjectParser<>)))
            .AsImplementedInterfaces()
            .WithTransientLifetime());

        services.Scan(tss => tss.FromAssemblyOf<TodoItemMapper>()
            .AddClasses(c => c.AssignableTo(typeof(IMapper<,>)))
            .AsImplementedInterfaces()
            .WithTransientLifetime());

        services.AddTransient<IUsersSheetsService, UsersSheetsService>()
            .AddTransient<ISuggestionsSheetsService, SuggestionsSheetsService>()
            .AddTransient<ITodoItemSheetsService, TodoItemSheetsService>();

        services.AddHostedService<EclipseApplicationInizializerHostedService>();

        services
            .Decorate<IReminderService, CachedReminderService>()
            .Decorate<IIdentityUserService, CachedIdentityUserService>();

        return services;
    }
}
