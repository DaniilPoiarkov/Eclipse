using Eclipse.Application.Contracts.Telegram;
using Eclipse.Common.Results;

using Telegram.Bot;

namespace Eclipse.Application.Telegram;

internal sealed class TelegramService : ITelegramService
{
    private readonly ITelegramBotClient _botClient;

    private static readonly string _errorSendCode = "Telegram.Send";

    private static readonly string _errorWebhookCode = "Telegram.Webhook";

    public TelegramService(ITelegramBotClient botClient)
    {
        _botClient = botClient;
    }

    public async Task<Result> Send(SendMessageModel message, CancellationToken cancellationToken = default)
    {
        if (message.Message.IsNullOrEmpty())
        {
            return Error.Validation(_errorSendCode, "Telegram:MessageCannotBeEmpty");
        }

        if (message.ChatId == default)
        {
            return Error.Validation(_errorSendCode, "Telegram:InvalidChatId");
        }

        await _botClient.SendTextMessageAsync(
            message.ChatId,
            message.Message,
            cancellationToken: cancellationToken);

        return Result.Success();
    }

    public async Task<Result> SetWebhookUrlAsync(string? webhookUrl, string secretToken, CancellationToken cancellationToken)
    {
        if (webhookUrl.IsNullOrEmpty())
        {
            return Error.Validation(_errorWebhookCode, "Telegram:WebhookEmpty");
        }

        try
        {
            await _botClient.SetWebhookAsync(
                webhookUrl,
                secretToken: secretToken,
                cancellationToken: cancellationToken
            );

            return Result.Success();
        }
        catch (Exception ex)
        {
            return Error.Failure(_errorWebhookCode, ex.Message);
        }
    }
}
