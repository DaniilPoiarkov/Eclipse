using Telegram.Bot;
using Telegram.Bot.Types;

namespace Eclipse.Core.Results;

/// <summary>
/// Represents result of pipeline execution which need to be send to the user
/// </summary>
public interface IResult
{
    /// <summary>
    /// Sends bot answer to the user
    /// </summary>
    /// <param name="botClient"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<Message?> SendAsync(ITelegramBotClient botClient, CancellationToken cancellationToken = default);
}
