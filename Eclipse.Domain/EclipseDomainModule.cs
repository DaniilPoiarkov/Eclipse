using Eclipse.Domain.IdentityUsers;

using Microsoft.Extensions.DependencyInjection;

namespace Eclipse.Domain;

/// <summary>
/// Takes responsibility for domain logic
/// </summary>
public static class EclipseDomainModule
{
    public static IServiceCollection AddDomainModule(this IServiceCollection services)
    {
        //TODO: Add domain services, managers etc
        services.AddTransient<IdentityUserManager>();

        return services;
    }
}
