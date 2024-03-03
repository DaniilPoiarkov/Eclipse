using Eclipse.Application.Sheets;
using Eclipse.Suggestions.Domain;

using Microsoft.Extensions.DependencyInjection;

namespace Eclipse.Suggestions.Persistence;

public static class SuggestionsPersistence
{
    public static IServiceCollection AddSuggestionsPersistence(this IServiceCollection services)
    {
        services
            .AddScoped<IEclipseSheetsService<Suggestion>, SuggestionsSheetsService>()
            .AddScoped<ISuggestionRepository, SuggestionRepository>();

        return services;
    }
}
