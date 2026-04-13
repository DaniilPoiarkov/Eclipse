using Eclipse.Core.Pipelines;
using Eclipse.Core.Routing;

using System.Reflection;

using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Eclipse.Core.Provider.Handlers;

internal sealed class PurchasedPaidMediaHandler : IRouteHandler
{
    private readonly IEnumerable<IPipeline> _pipelines;

    public PurchasedPaidMediaHandler(IEnumerable<IPipeline> pipelines)
    {
        _pipelines = pipelines;
    }

    public UpdateType Type => UpdateType.PurchasedPaidMedia;

    public IPipeline? Get(Update update)
    {
        return _pipelines.FirstOrDefault(p => p.GetType()
            .GetCustomAttribute<PurchasedPaidMediaPipelineAttribute>() is not null
        );
    }
}
