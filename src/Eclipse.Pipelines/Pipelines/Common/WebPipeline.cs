using Eclipse.Application.Contracts.Url;
using Eclipse.Core.Attributes;
using Eclipse.Pipelines.Attributes;

namespace Eclipse.Pipelines.Pipelines.Common;

[ComingSoonFeature]
[Route("", "/web")]
internal sealed class WebPipeline : EclipsePipelineBase
{
    private readonly IAppUrlProvider _appUrlProvider;

    public WebPipeline(IAppUrlProvider appUrlProvider)
    {
        _appUrlProvider = appUrlProvider;
    }

    protected override void Initialize()
    {
        RegisterStage((context) => Text($"{_appUrlProvider.AppUrl.EnsureEndsWith('/')}swagger/index.html"));
    }
}
