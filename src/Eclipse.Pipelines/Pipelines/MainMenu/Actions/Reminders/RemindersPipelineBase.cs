using Telegram.Bot.Types.ReplyMarkups;

namespace Eclipse.Pipelines.Pipelines.MainMenu.Actions.Reminders;

internal abstract class RemindersPipelineBase : ActionsPipelineBase
{
    protected IReadOnlyList<IReadOnlyList<KeyboardButton>> RemindersMenuButtons => new List<KeyboardButton[]>
    {
        new[] { new KeyboardButton(Localizer["Menu:Reminders:Add"]) },
        new[] { new KeyboardButton(Localizer["Menu:MainMenu:Actions"]) },
    };
}
