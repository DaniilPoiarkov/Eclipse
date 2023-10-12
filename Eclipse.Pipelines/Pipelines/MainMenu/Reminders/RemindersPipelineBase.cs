using Telegram.Bot.Types.ReplyMarkups;

namespace Eclipse.Pipelines.Pipelines.MainMenu.Reminders;

public abstract class RemindersPipelineBase : EclipsePipelineBase
{
    protected IReadOnlyList<KeyboardButton> RemindersMenuButtons => new KeyboardButton[]
    {
        new KeyboardButton(Localizer["Menu:Reminders:Add"]),
        new KeyboardButton(Localizer["Menu:MainMenu"]),
    };

    protected static bool TryParseTime(string value, out TimeOnly parsed)
    {
        parsed = default;

        var values = value.Split(':').ToArray();

        if (values.Length != 2)
        {
            return false;
        }

        if (!TryParse(values[0], out var hours) || !TryParse(values[1], out var minutes))
        {
            return false;
        }

        parsed = new TimeOnly(hours, minutes);

        return true;

        static bool TryParse(string value, out int num) => int.TryParse(value, out num);
    }
}
