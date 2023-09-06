using Eclipse.Application.Contracts.Telegram.Pipelines;
using Eclipse.Application.Contracts.Telegram.TelegramUsers;
using Eclipse.Application.Telegram.Pipelines;
using Eclipse.Application.Telegram.TelegramUsers;
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
            .AddTransient<IPipelineStore, PipelineStore>();

        return services;
    }
}
