﻿using Eclipse.Application.Contracts.Telegram;
using Eclipse.Common.Results;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Localization;

using Telegram.Bot;

namespace Eclipse.Application.Telegram;

internal sealed class TelegramService : ITelegramService
{
    private readonly ITelegramBotClient _botClient;

    private readonly IConfiguration _configuration;

    private readonly IStringLocalizer<TelegramService> _localizer;

    private static readonly string _errorSendCode = "Telegram.Send";

    private static readonly string _errorWebhookCode = "Telegram.Webhook";

    public TelegramService(ITelegramBotClient botClient, IConfiguration configuration, IStringLocalizer<TelegramService> localizer)
    {
        _botClient = botClient;
        _configuration = configuration;
        _localizer = localizer;
    }

    public async Task<Result> Send(SendMessageModel message, CancellationToken cancellationToken = default)
    {
        if (message.Message.IsNullOrEmpty())
        {
            return Error.Validation(_errorSendCode, _localizer["Telegram:MessageCannotBeEmpty"]);
        }

        if (message.ChatId == default)
        {
            return Error.Validation(_errorSendCode, _localizer["Telegram:InvalidChatId"]);
        }

        await _botClient.SendTextMessageAsync(
            message.ChatId,
            message.Message,
            cancellationToken: cancellationToken);

        return Result.Success();
    }

    public async Task<Result> SetWebhookUrlAsync(string? webhookUrl, CancellationToken cancellationToken = default)
    {
        if (webhookUrl.IsNullOrEmpty() || !webhookUrl.StartsWith("https"))
        {
            return Error.Validation(_errorWebhookCode, _localizer["Telegram:WebhookInvalid"]);
        }

        try
        {
            var secretToken = _configuration["Telegram:SecretToken"];

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
