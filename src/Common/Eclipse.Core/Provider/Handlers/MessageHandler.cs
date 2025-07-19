using Eclipse.Core.Keywords;
using Eclipse.Core.Pipelines;
using Eclipse.Core.Routing;

using System.Reflection;

using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Eclipse.Core.Provider.Handlers;

internal sealed class MessageHandler : IRouteHandler
{
    public UpdateType Type => UpdateType.Message;

    private readonly IEnumerable<PipelineBase> _pipelines;

    private readonly IKeywordMapper _keywordMapper;

    public MessageHandler(IEnumerable<PipelineBase> pipelines, IKeywordMapper keywordMapper)
    {
        _pipelines = pipelines;
        _keywordMapper = keywordMapper;
    }

    public PipelineBase? Get(Update update)
    {
        var message = update.Message
            ?? throw new InvalidOperationException("Message is null.");

        var text = message.Text;

        if (string.IsNullOrEmpty(text))
        {
            return null;
        }

        var route = _keywordMapper.Map(text);

        if (route.StartsWith('/'))
        {
            return _pipelines.FirstOrDefault(p => p.GetType().GetCustomAttribute<RouteAttribute>() is RouteAttribute attribute
                && attribute.Command == route);
        }

        return _pipelines.FirstOrDefault(p => p.GetType().GetCustomAttribute<RouteAttribute>() is RouteAttribute attribute
            && attribute.Route == route);
    }
}
