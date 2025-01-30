using Eclipse.Core.Attributes;

namespace Eclipse.Pipelines.Pipelines.AdminMenu.Promotions;

[Route("Menu:AdminMenu:Promotions", "/admin_promotions")]
internal sealed class PromotionsPipeline : AdminPipelineBase
{
    protected override void Initialize()
    {
        RegisterStage(context => Menu(PromotionsButtons, Localizer["Pipelines:Admin:Promotions"]));
    }
}
