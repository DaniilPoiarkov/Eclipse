using Eclipse.Pipelines.Attributes;

using Telegram.Bot.Types.ReplyMarkups;

namespace Eclipse.Pipelines.Pipelines;

[AdminOnly]
public abstract class AdminPipelineBase : EclipsePipelineBase
{
    protected static IReadOnlyCollection<IReadOnlyCollection<KeyboardButton>> AdminMenuButtons => new List<KeyboardButton[]>
    {
        new[] { new KeyboardButton(Localizer["Menu:AdminMenu:ViewSuggestions"]), new KeyboardButton(Localizer["Menu:AdminMenu:ViewUsers"]) },
        new[] { new KeyboardButton(Localizer["Menu:AdminMenu:SendToUser"]), new KeyboardButton(Localizer["Menu:AdminMenu:SendToAll"]) },
        new[] { new KeyboardButton(Localizer["Menu:AdminMenu:SwitchToUserMode"]) },
    };
}
