using Eclipse.Core.Routing;

namespace Eclipse.Pipelines.Pipelines.MainMenu.ApiTokens;

[Route("Menu:ApiTokens", "/api_tokens")]
internal sealed class ApiTokensPipeline : ApiTokensPipelineBase
{
    protected override void Initialize()
    {
        RegisterStage(_ => Menu(ApiTokensMenuButtons, Localizer["Pipelines:ApiTokens"]));
    }
}
