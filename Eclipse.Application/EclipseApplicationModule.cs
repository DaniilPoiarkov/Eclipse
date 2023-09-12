using Eclipse.Application.Contracts.Google.Sheets;
using Eclipse.Application.Contracts.Google.Sheets.Suggestions;
using Eclipse.Application.Contracts.Google.Sheets.Users;
using Eclipse.Application.Contracts.Suggestions;
using Eclipse.Application.Contracts.Telegram.Commands;
using Eclipse.Application.Contracts.Telegram.Pipelines;
using Eclipse.Application.Contracts.Telegram.TelegramUsers;
using Eclipse.Application.Google.Sheets;
using Eclipse.Application.Google.Sheets.Parsers;
using Eclipse.Application.Google.Sheets.Suggestions;
using Eclipse.Application.Google.Sheets.Users;
using Eclipse.Application.Suggestions;
using Eclipse.Application.Telegram.Commands;
using Eclipse.Application.Telegram.Pipelines;
using Eclipse.Application.Telegram.TelegramUsers;
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
            .AddSingleton<ITelegramUserStore, TelegramUserStore>()
            .AddSingleton<IPipelineStore, PipelineStore>()
            .AddTransient<ICommandService, CommandService>()
            .AddTransient<ITelegramUserRepository, TelegramUserRepository>()
            .AddTransient<ISuggestionsService, SuggestionsService>();

        services.AddValidatorsFromAssemblyContaining<CommandDtoValidator>(ServiceLifetime.Transient);

        services.Scan(tss => tss.FromAssemblyOf<SuggestionObjectParser>()
            .AddClasses(c => c.AssignableTo(typeof(IObjectParser<>)))
            .AsImplementedInterfaces()
            .WithTransientLifetime());

        services.AddTransient<IUsersSheetsService, UsersSheetsService>()
            .AddTransient<ISuggestionsSheetsService, SuggestionsSheetsService>();

        return services;
    }
}
