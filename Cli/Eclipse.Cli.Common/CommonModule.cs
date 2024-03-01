using Eclipse.Cli.Common.Disk;
using Eclipse.Cli.Common.IO;
using Eclipse.Cli.Common.IO.Console;

using Microsoft.Extensions.DependencyInjection;

namespace Eclipse.Cli.Common;

public static class CommonModule
{
    public static IServiceCollection AddCommon(this IServiceCollection services)
    {
        services
            .AddSingleton<IInputReader, ConsoleReader>()
            .AddSingleton<IOutputWriter, ConsoleWriter>()
            .AddSingleton<IWindow, ConsoleWindow>();

        services.AddSingleton<IDisk, Disk.Disk>();



        return services;
    }
}
