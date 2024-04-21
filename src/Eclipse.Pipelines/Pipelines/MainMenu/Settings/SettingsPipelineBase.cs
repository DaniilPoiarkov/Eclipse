using Eclipse.Core.Core;

using Telegram.Bot.Types.ReplyMarkups;

namespace Eclipse.Pipelines.Pipelines.MainMenu.Settings;

public abstract class SettingsPipelineBase : EclipsePipelineBase
{
    protected IEnumerable<IEnumerable<KeyboardButton>> SettingsMenuButtons => new List<IList<KeyboardButton>>
    {
        new[] { new KeyboardButton(Localizer["Menu:Settings:Language"]), new KeyboardButton(Localizer["Menu:Settings:SetGmt"]) },
        new[] { new KeyboardButton(Localizer["Menu:Settings:Notifications"]) },
        new[] { new KeyboardButton(Localizer["Menu:MainMenu"]) }
    };

    private IResult MenuAndEdit(string message, int messageId)
    {
        var menu = Menu(SettingsMenuButtons, message);
        var edit = Edit(messageId, InlineKeyboardMarkup.Empty());

        return Multiple(menu, edit);
    }

    protected IResult MenuAndRemoveOptions(string message, int? messageId)
    {
        return messageId is null
            ? Menu(SettingsMenuButtons, message)
            : MenuAndEdit(message, messageId.Value);
    }
}
