using Eclipse.Application.Contracts.Telegram;
using Eclipse.Application.Contracts.UserStores;
using Eclipse.Application.Telegram;
using Eclipse.Application.UserStores;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Eclipse.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.TryAddTransient<IUserStore, UserStore>();
        services.TryAddTransient<ITelegramUpdateHandler, TelegramUpdateHandler>();

        return services;
    }
}
