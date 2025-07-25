﻿using Telegram.Bot;
using Telegram.Bot.Types;

namespace Eclipse.Core.Results;

public sealed class TextResult : ResultBase
{
    public string Message { get; }

    public TextResult(string message)
    {
        Message = message;
    }

    public override async Task<Message?> SendAsync(ITelegramBotClient botClient, CancellationToken cancellationToken = default)
    {
        return await botClient.SendMessage(ChatId, Message, cancellationToken: cancellationToken);
    }
}
