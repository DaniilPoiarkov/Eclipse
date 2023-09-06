using Eclipse.Application.Contracts.Telegram;
using Eclipse.Application.Contracts.Telegram.Stores;
using Eclipse.Application.Telegram;
using Eclipse.Application.Telegram.Stores;
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
            .AddTransient<IUserInfoStore, UserInfoStore>()
            .AddTransient<IPipelineStore, PipelineStore>()
            .AddTransient<ITelegramUpdateHandler, TelegramUpdateHandler>();

        return services;
    }
}
