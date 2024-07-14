using Eclipse.Core.Attributes;
using Eclipse.Core.Core;

namespace Eclipse.Pipelines.Pipelines.Common;

[Route("", "/about")]
public sealed class AboutPipeline : EclipsePipelineBase
{
    protected override void Initialize()
    {
        RegisterStage(SendAbout);
    }

    public IResult SendAbout(MessageContext context)
    {
        var about = Localizer["Pipelines:Common:About"].Value
            .Replace("{name}", context.User.Name.Trim());

        return Text(about);
    }
}
