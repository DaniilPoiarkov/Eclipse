using Eclipse.Core.Routing;

namespace Eclipse.Pipelines.Pipelines.MainMenu.Settings;

[Route("Menu:MainMenu:Settings", "/settings")]
internal sealed class SettingsPipeline : SettingsPipelineBase
{
    protected override void Initialize()
    {
        RegisterStage(_ => Menu(SettingsMenuButtons, Localizer["Pipelines:Settings"]));
    }
}
