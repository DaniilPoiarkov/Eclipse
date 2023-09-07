using Eclipse.Core.Attributes;
using Eclipse.Core.Core;
using Microsoft.Extensions.Configuration;

namespace Eclipse.Pipelines.Pipelines;

[Route("", "/about")]
public sealed class AboutPipeline : EclipsePipelineBase
{
    private static readonly string _default = "Hi, {name}!\r\n" +
        "I'm Eclipse. Use /help to get some addition info";

    private readonly IConfiguration _configuration;

    public AboutPipeline(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    protected override void Initialize()
    {
        RegisterStage(SendAbout);
    }

    public IResult SendAbout(MessageContext context)
    {
        var about = _configuration["Eclipse:About"]
            ?? _default.Replace("{name}", context.User.Name);

        return Text(about);
    }
}
