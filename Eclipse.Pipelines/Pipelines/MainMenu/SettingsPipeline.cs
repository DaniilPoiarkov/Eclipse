using Eclipse.Core.Attributes;
using Eclipse.Pipelines.Pipelines.MainMenu.Settings;

namespace Eclipse.Pipelines.Pipelines.MainMenu;

[Route("Menu:MainMenu:Settings", "/settings")]
internal sealed class SettingsPipeline : SettingsPipelineBase
{
    protected override void Initialize()
    {
        RegisterStage(_ => Menu(SettingsMenuButtons, Localizer["Pipelines:Settings"]));
    }
}
