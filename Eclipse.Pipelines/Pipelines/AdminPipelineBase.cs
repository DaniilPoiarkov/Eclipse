using Eclipse.Pipelines.Attributes;

using Telegram.Bot.Types.ReplyMarkups;

namespace Eclipse.Pipelines.Pipelines;

[AdminOnly]
public abstract class AdminPipelineBase : EclipsePipelineBase
{
    protected static IReadOnlyCollection<IReadOnlyCollection<KeyboardButton>> AdminMenuButtons => new List<KeyboardButton[]>
    {
        new[] { new KeyboardButton(Localizer["Menu:AdminMenu:View"]), new KeyboardButton(Localizer["Menu:AdminMenu:Send"]) },
        //new[] { new KeyboardButton(Localizer["Menu:AdminMenu:Morning"]) },
        new[] { new KeyboardButton(Localizer["Menu:AdminMenu:SwitchToUserMode"]) },
    };

    protected static IReadOnlyCollection<IReadOnlyCollection<KeyboardButton>> SendButtons => new List<KeyboardButton[]>()
    {
        new[] { new KeyboardButton(Localizer["Menu:AdminMenu:Send:User"]), new KeyboardButton(Localizer["Menu:AdminMenu:Send:All"]) },
        new[] { new KeyboardButton(Localizer["Menu:AdminMenu"]) }
    };

    protected static IReadOnlyCollection<IReadOnlyCollection<KeyboardButton>> ViewButtons => new List<KeyboardButton[]>()
    {
        new[] { new KeyboardButton(Localizer["Menu:AdminMenu:View:Suggestions"]), new KeyboardButton(Localizer["Menu:AdminMenu:View:Users"]) },
        new[] { new KeyboardButton(Localizer["Menu:AdminMenu"]) }
    };
}
