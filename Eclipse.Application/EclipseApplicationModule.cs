using Eclipse.Application.Contracts.Telegram.Commands;
using Eclipse.Application.Contracts.Telegram.Pipelines;
using Eclipse.Application.Contracts.Telegram.TelegramUsers;
using Eclipse.Application.Google.Sheets;
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
            .AddSingleton<ITelegramUserStore, TelegramUserStore>()
            .AddSingleton<IPipelineStore, PipelineStore>()
            .AddTransient<ICommandService, CommandService>()
            .AddTransient<ITelegramUserRepository, TelegramUserRepository>();

        services.AddValidatorsFromAssemblyContaining<CommandDtoValidator>(ServiceLifetime.Transient);

        services.AddTransient<EclipseSheetsServiceFactory>()
            .AddTransient(sp => sp.GetRequiredService<EclipseSheetsServiceFactory>().Build());

        return services;
    }
}
