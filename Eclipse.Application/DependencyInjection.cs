using Eclipse.Application.Contracts.UserStores;
using Eclipse.Application.UserStores;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Eclipse.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.TryAddTransient<IUserStore, UserStore>();

        return services;
    }
}
