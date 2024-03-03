using Eclipse.Suggestions.Application.Contracts;

using Microsoft.Extensions.DependencyInjection;

namespace Eclipse.Suggestions.Application;

public static class SuggestionsApplication
{
    public static IServiceCollection AddSuggestionsApplication(this IServiceCollection services)
    {
        services
            .AddTransient<ISuggestionService, SuggestionService>();

        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(typeof(SuggestionsApplication).Assembly);
        });

        return services;
    }
}
