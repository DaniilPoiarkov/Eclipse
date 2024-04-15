using Telegram.Bot.Types.ReplyMarkups;

namespace Eclipse.Pipelines.Pipelines.MainMenu.Reminders;

public abstract class RemindersPipelineBase : EclipsePipelineBase
{
    protected IReadOnlyList<IReadOnlyList<KeyboardButton>> RemindersMenuButtons => new List<KeyboardButton[]>
    {
        new[] { new KeyboardButton(Localizer["Menu:Reminders:Add"]) },
        new[] { new KeyboardButton(Localizer["Menu:MainMenu"]) },
    };
}
