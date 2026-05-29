using Eclipse.MCP.Core.Client;

namespace Eclipse.MCP.Requests.Telegram;

internal static class TelegramExtensions
{
    public static Task<EclipseResponse<string>> SendTelegramMessageAsync(this IEclipseClient client, string message, long chatId, CancellationToken cancellationToken = default)
        => client.SendRequestAsync(new SendTelegramMessageRequest(message, chatId), cancellationToken);

    public static Task<EclipseResponse<string>> SwitchHandlerAsync(this IEclipseClient client, string type, CancellationToken cancellationToken = default)
        => client.SendRequestAsync(new SwitchHandlerRequest(type), cancellationToken);
}
