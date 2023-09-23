using Eclipse.Application.Contracts.Localizations;
using Eclipse.Core.Pipelines;
using Eclipse.Pipelines.CachedServices;

using Microsoft.Extensions.DependencyInjection;

using Telegram.Bot.Types.ReplyMarkups;

namespace Eclipse.Pipelines.Pipelines;

public abstract class EclipsePipelineBase : PipelineBase
{
    private static readonly Lazy<IEclipseLocalizer> _localizer = new(GetService<IEclipseLocalizer>);
    protected static IEclipseLocalizer Localizer => _localizer.Value;

    protected static IReadOnlyCollection<KeyboardButton> MainMenuButtons => new KeyboardButton[]
    {
        new KeyboardButton(Localizer["Menu:MainMenu:Suggest"]),
        new KeyboardButton(Localizer["Menu:MainMenu:MyToDos"]),
        new KeyboardButton(Localizer["Menu:MainMenu:Language"]),
    };

    private static TService GetService<TService>()
        where TService : class
    {
        using var scope = CachedServiceProvider.Services.CreateScope();
        return scope.ServiceProvider.GetRequiredService<TService>();
    }
}
