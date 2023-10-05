using Eclipse.Domain.IdentityUsers;
using Eclipse.Domain.Reminders;

using Microsoft.Extensions.DependencyInjection;

namespace Eclipse.Domain;

/// <summary>
/// Takes responsibility for domain logic
/// </summary>
public static class EclipseDomainModule
{
    public static IServiceCollection AddDomainModule(this IServiceCollection services)
    {
        services
            .AddTransient<IdentityUserManager>()
            .AddTransient<ReminderManager>();

        return services;
    }
}
