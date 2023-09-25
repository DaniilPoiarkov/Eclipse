using Eclipse.Core.Attributes;

namespace Eclipse.Pipelines.Pipelines.AdminMenu.SendMessage;

[Route("Menu:AdminMenu:Send", "/admin_send")]
internal class AdminSendPipeline : AdminPipelineBase
{
    protected override void Initialize()
    {
        RegisterStage(_ => Menu(SendButtons, Localizer["Pipelines:AdminMenu:Send"]));
    }
}
