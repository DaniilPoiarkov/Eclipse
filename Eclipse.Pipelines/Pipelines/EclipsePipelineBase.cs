using Eclipse.Application.Contracts.Localizations;
using Eclipse.Core.Pipelines;

using Telegram.Bot.Types.ReplyMarkups;

namespace Eclipse.Pipelines.Pipelines;

public abstract class EclipsePipelineBase : PipelineBase
{
    protected IEclipseLocalizer Localizer { get; private set; } = null!;
    //{
    //    get
    //    {
    //        var currentUser = GetService<ICurrentTelegramUser>().GetCurrentUser();
    //        var localizer = GetService<IEclipseLocalizer>();
            
    //        if (currentUser is not null)
    //        {
    //            localizer.CheckCulture(currentUser.Id);
    //        }

    //        return localizer;
    //    }
    //}

    protected IReadOnlyCollection<IReadOnlyCollection<KeyboardButton>> MainMenuButtons => new List<KeyboardButton[]>
    {
        new[] { new KeyboardButton(Localizer["Menu:MainMenu:MyToDos"]), new KeyboardButton(Localizer["Menu:MainMenu:Reminders"]) },
        new[] { new KeyboardButton(Localizer["Menu:MainMenu:Suggest"]), new KeyboardButton(Localizer["Menu:MainMenu:Settings"]) }
    };

    //private static TService GetService<TService>()
    //    where TService : class
    //{
    //    return CachedServiceProvider.Services.GetRequiredService<TService>();
    //}

    internal void SetLocalizer(IEclipseLocalizer localizer)
    {
        Localizer = localizer;
    }
}
