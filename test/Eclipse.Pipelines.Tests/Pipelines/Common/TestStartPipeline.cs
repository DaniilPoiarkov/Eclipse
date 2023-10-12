using Eclipse.Application.Contracts.Localizations;
using Eclipse.Core.Core;
using Eclipse.Pipelines.CachedServices;
using Eclipse.Pipelines.Pipelines.Common;

using NSubstitute;

namespace Eclipse.Pipelines.Tests.Pipelines.Common;

internal class TestStartPipeline : StartPipeline
{
    internal readonly IEclipseLocalizer ActualLocalizer;

    public TestStartPipeline()
    {
        ActualLocalizer = Substitute.For<IEclipseLocalizer>();
        Localizer.ReturnsForAnyArgs(ActualLocalizer);
    }

    protected override IResult Start(MessageContext context)
    {
        return base.Start(context);
    }
}
