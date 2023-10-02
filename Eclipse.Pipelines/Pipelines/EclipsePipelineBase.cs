using Eclipse.Application.Contracts.Localizations;
using Eclipse.Core.Core;
using Eclipse.Core.Pipelines;
using Eclipse.Pipelines.CachedServices;

using Microsoft.Extensions.DependencyInjection;

using Telegram.Bot.Types.ReplyMarkups;

namespace Eclipse.Pipelines.Pipelines;

public abstract class EclipsePipelineBase : PipelineBase
{
    protected static IEclipseLocalizer Localizer
    {
        get
        {
            var currentUser = GetService<ICurrentTelegramUser>().GetCurrentUser();
            var localizer = GetService<IEclipseLocalizer>();
            
            if (currentUser is not null)
            {
                localizer.CheckCulture(currentUser.Id);
            }

            return localizer;
        }
    }

    protected static IReadOnlyCollection<KeyboardButton> MainMenuButtons => new KeyboardButton[]
    {
        new KeyboardButton(Localizer["Menu:MainMenu:Suggest"]),
        new KeyboardButton(Localizer["Menu:MainMenu:MyToDos"]),
        new KeyboardButton(Localizer["Menu:MainMenu:Language"]),
    };

    private static TService GetService<TService>()
        where TService : class
    {
        return CachedServiceProvider.Services.GetRequiredService<TService>();
    }
}
