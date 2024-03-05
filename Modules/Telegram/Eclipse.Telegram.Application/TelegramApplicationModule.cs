using Eclipse.Telegram.Application.Commands;
using Eclipse.Telegram.Application.Contracts;
using Eclipse.Telegram.Application.Contracts.Commands;

using Microsoft.Extensions.DependencyInjection;

namespace Eclipse.Telegram.Application;

public static class TelegramApplicationModule
{
    public static IServiceCollection AddTelegramApplication(this IServiceCollection services)
    {
        services
            .AddTransient<ITelegramService, TelegramService>()
            .AddTransient<ICommandService, CommandService>();

        return services;
    }
}
