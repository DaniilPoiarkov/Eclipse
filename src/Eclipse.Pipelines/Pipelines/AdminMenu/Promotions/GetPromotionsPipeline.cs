using Eclipse.Application.Contracts.Promotions;
using Eclipse.Core.Routing;

namespace Eclipse.Pipelines.Pipelines.AdminMenu.Promotions;

[Route("Menu:AdminMenu:Promotions:Get", "/admin_promotions_get")]
internal sealed class GetPromotionsPipeline : AdminPipelineBase
{
    private readonly IPromotionService _promotionService;

    protected override void Initialize()
    {
        
    }
}
