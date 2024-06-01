using Telegram.Bot.Polling;

namespace Eclipse.Pipelines.UpdateHandler;

public interface IEclipseUpdateHandler : IUpdateHandler
{
    HandlerType Type { get; }
}
