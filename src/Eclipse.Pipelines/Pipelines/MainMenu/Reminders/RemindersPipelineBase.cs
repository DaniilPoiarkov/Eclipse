using Telegram.Bot.Types.ReplyMarkups;

namespace Eclipse.Pipelines.Pipelines.MainMenu.Reminders;

internal abstract class RemindersPipelineBase : EclipsePipelineBase
{
    protected IReadOnlyList<IReadOnlyList<KeyboardButton>> RemindersMenuButtons => new List<KeyboardButton[]>
    {
        new[] { new KeyboardButton(Localizer["Menu:Reminders:Add"]), new KeyboardButton(Localizer["Menu:Reminders:List"]) },
        new[] { new KeyboardButton(Localizer["Menu:MainMenu"]) },
    };
}
