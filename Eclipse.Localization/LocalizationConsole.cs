using Eclipse.Localization.Builder;
using Eclipse.Localization.Exceptions;
using Eclipse.Localization.Localizers;

using Microsoft.Extensions.DependencyInjection;

namespace Eclipse.Localization;

/// <summary>
/// Used only for testing purposes
/// </summary>
internal class LocalizationConsole
{
    public static void Main()
    {
        var services = new ServiceCollection();

        services.AddLocalizationSupport(builder =>
        {
            builder.AddJsonFiles("Resources");

            builder.DefaultLocalization = "ua";
        });

        var localizer = services.BuildServiceProvider()
            .GetRequiredService<ILocalizer>();

        try
        {
            _ = new LocalizationBuilder()
                .AddJsonFile("fr.json");
        }
        catch (LocalizedException ex)
        {
            var message = string.Format(localizer[ex.Message, "en"], ex.Args);
            Console.WriteLine(message);
        }

        Console.WriteLine();
    }
}
