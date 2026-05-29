using Eclipse.Core.Routing;

using Telegram.Bot.Types.ReplyMarkups;

namespace Eclipse.Pipelines.Pipelines.MainMenu.ApiTokens;

[Route("Menu:ApiTokens", "/api_tokens")]
internal sealed class ApiTokensPipeline : EclipsePipelineBase
{
    private IEnumerable<IEnumerable<KeyboardButton>> ApiTokensMenuButtons =>
    [
        [new KeyboardButton(Localizer["Menu:ApiTokens:Create"]), new KeyboardButton(Localizer["Menu:ApiTokens:List"])],
        [new KeyboardButton(Localizer["Menu:MainMenu"])]
    ];

    protected override void Initialize()
    {
        RegisterStage(_ => Menu(ApiTokensMenuButtons, Localizer["Pipelines:ApiTokens"]));
    }
}
