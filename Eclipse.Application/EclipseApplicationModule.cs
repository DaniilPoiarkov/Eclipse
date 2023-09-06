using Eclipse.Application.Contracts.Telegram.BotManagement;
using Eclipse.Application.Contracts.Telegram.Pipelines;
using Eclipse.Application.Contracts.Telegram.TelegramUsers;
using Eclipse.Application.Telegram.Commands;
using Eclipse.Application.Telegram.Pipelines;
using Eclipse.Application.Telegram.TelegramUsers;
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
            .AddTransient<ITelegramUserStore, TelegramUserStore>()
            .AddTransient<IPipelineStore, PipelineStore>()
            .AddTransient<ICommandService, CommandService>();

        services.AddValidatorsFromAssemblyContaining<CommandDtoValidator>();
        
        // TODO: Why the hell call above not registering this validator
        services.AddTransient<IValidator<CommandDto>, CommandDtoValidator>();

        return services;
    }
}
