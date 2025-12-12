using Eclipse.Core.Routing;

namespace Eclipse.Pipelines.Pipelines.AdminMenu.Feedbacks;

[Route("Menu:AdminMenu:Feedbacks", "/admin_feedbacks")]
internal sealed class FeedbacksPipeline : AdminPipelineBase
{
    protected override void Initialize()
    {
        RegisterStage(_ => Menu(FeedbacksButtons, Localizer["Pipelines:Admin:Feedbacks"]));
    }
}
