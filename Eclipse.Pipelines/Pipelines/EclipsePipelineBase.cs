using Eclipse.Core.Pipelines;
using Eclipse.Localization.Localizers;
using Eclipse.Pipelines.CachedServices;

using Microsoft.Extensions.DependencyInjection;

using Telegram.Bot.Types.ReplyMarkups;

namespace Eclipse.Pipelines.Pipelines;

public abstract class EclipsePipelineBase : PipelineBase
{
    protected static ILocalizer Localizer => GetService<ILocalizer>();

    protected static IReadOnlyCollection<KeyboardButton> MainMenuButtons => new KeyboardButton[]
    {
        new KeyboardButton(Localizer["Menu:MainMenu:Suggest"]),
        new KeyboardButton(Localizer["Menu:MainMenu:MyToDos"])
    };

    private static TService GetService<TService>()
        where TService : class
    {
        using var scope = CachedServiceProvider.Services.CreateScope();
        return scope.ServiceProvider.GetRequiredService<TService>();
    }
}
