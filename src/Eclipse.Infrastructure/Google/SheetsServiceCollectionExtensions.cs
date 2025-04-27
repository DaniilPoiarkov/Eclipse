using Eclipse.Common.Sheets;

using Microsoft.Extensions.DependencyInjection;

namespace Eclipse.Infrastructure.Google;

public static class SheetsServiceCollectionExtensions
{
    public static IServiceCollection UseGoogleSheets(this IServiceCollection services)
    {
        services.AddOptions<GoogleOptions>()
            .BindConfiguration("Google")
            .ValidateOnStart();

        services
            .AddSingleton<IGoogleClient, GoogleClient>()
            .AddSingleton(sp => sp.GetRequiredService<IGoogleClient>().GetSheetsService())
                .AddScoped<ISheetsService, GoogleSheetsService>();

        return services;
    }
}
