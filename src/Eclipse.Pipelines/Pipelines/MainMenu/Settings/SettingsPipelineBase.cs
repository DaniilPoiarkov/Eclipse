using Telegram.Bot.Types.ReplyMarkups;

namespace Eclipse.Pipelines.Pipelines.MainMenu.Settings;

public abstract class SettingsPipelineBase : EclipsePipelineBase
{
    protected IEnumerable<IEnumerable<KeyboardButton>> SettingsMenuButtons => new List<IList<KeyboardButton>>
    {
        new[] { new KeyboardButton(Localizer["Menu:Settings:Language"]), new KeyboardButton(Localizer["Menu:Settings:SetGmt"]) },
        new[] { new KeyboardButton(Localizer["Menu:MainMenu"]) }
    };
}
