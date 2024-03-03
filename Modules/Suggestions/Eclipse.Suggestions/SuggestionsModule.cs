using Eclipse.Suggestions.Application;
using Eclipse.Suggestions.Persistence;

using Microsoft.Extensions.DependencyInjection;

namespace Eclipse.Suggestions;

public static class SuggestionsModule
{
    public static IServiceCollection AddSuggestionsModule(this IServiceCollection services)
    {
        services
            .AddSuggestionsPersistence()
            .AddSuggestionsApplication();

        return services;
    }
}
