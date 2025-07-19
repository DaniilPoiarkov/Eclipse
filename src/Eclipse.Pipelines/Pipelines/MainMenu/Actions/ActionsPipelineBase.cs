using Telegram.Bot.Types.ReplyMarkups;

namespace Eclipse.Pipelines.Pipelines.MainMenu.Actions;

internal abstract class ActionsPipelineBase : EclipsePipelineBase
{
    protected IReadOnlyList<IReadOnlyList<KeyboardButton>> ActionsMenuButtons => new List<KeyboardButton[]>
    {
        new[] { new KeyboardButton(Localizer["Menu:MainMenu:MyToDos"]), new KeyboardButton(Localizer["Menu:MainMenu:Reminders"]) },
        new[] { new KeyboardButton(Localizer["Menu:MainMenu"]) },
    };
}
