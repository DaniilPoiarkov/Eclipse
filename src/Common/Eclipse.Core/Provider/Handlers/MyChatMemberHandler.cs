using Eclipse.Core.Pipelines;
using Eclipse.Core.Routing;

using System.Reflection;

using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Eclipse.Core.Provider.Handlers;

internal sealed class MyChatMemberHandler : IRouteHandler
{
    private readonly IEnumerable<IPipeline> _pipelines;

    public MyChatMemberHandler(IEnumerable<IPipeline> pipelines)
    {
        _pipelines = pipelines;
    }

    public UpdateType Type => UpdateType.MyChatMember;

    public IPipeline? Get(Update update)
    {
        return _pipelines.FirstOrDefault(p =>
            p.GetType()
            .GetCustomAttributes<MyChatMemberPipelineAttribute>()
            .Any(a => a.CanHandle(update))
        );
    }
}
