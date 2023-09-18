using Eclipse.Core.Attributes;
using Eclipse.Core.Core;

namespace Eclipse.Pipelines.Pipelines.Common;

[Route("Start", "/start")]
public class StartPipeline : EclipsePipelineBase
{
    private static readonly string _welcomeMessage = "Hello, {name}, I'm Eclipse. " +
        "Right now I'm studing different things, so if you don't mind to help me become better you can press \'Suggest\' button and describe your thoughts.\nSee you 🌒";

    protected override void Initialize()
    {
        RegisterStage(Start);
    }

    protected virtual IResult Start(MessageContext context)
    {
        var message = _welcomeMessage.Replace("{name}", context.User.Name.TrimEnd());
        return Menu(MainMenuButtons, message);
    }
}
