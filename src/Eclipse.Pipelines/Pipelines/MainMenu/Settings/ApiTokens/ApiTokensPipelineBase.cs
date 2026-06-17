using Telegram.Bot.Types.ReplyMarkups;

namespace Eclipse.Pipelines.Pipelines.MainMenu.Settings.ApiTokens;

internal abstract class ApiTokensPipelineBase : EclipsePipelineBase
{
    protected IEnumerable<IEnumerable<KeyboardButton>> ApiTokensMenuButtons =>
    [
        [new KeyboardButton(Localizer["Menu:ApiTokens:Create"]), new KeyboardButton(Localizer["Menu:ApiTokens:List"])],
        [new KeyboardButton(Localizer["Menu:MainMenu:Settings"])]
    ];
}
