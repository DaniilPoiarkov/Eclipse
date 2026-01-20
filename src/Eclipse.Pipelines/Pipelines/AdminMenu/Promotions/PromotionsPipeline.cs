using Eclipse.Core.Routing;

namespace Eclipse.Pipelines.Pipelines.AdminMenu.Promotions;

[Route("Menu:Admin:Promotions", "/admin_promotions")]
internal sealed class PromotionsPipeline : AdminPipelineBase
{
    protected override void Initialize()
    {
        RegisterStage(context => Menu(PromotionsButtons, Localizer["Pipelines:Admin:Promotions"]));
    }
}
