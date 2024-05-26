using Eclipse.Common.Sheets;
using Eclipse.Domain.Users;
using Eclipse.Domain.Suggestions;

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
            .AddTransient<UserManager>()
            .AddSingleton<IObjectParser<Suggestion>, SuggestionParser>();

        return services;
    }
}
