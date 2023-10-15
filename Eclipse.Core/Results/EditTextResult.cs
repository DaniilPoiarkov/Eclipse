﻿using Telegram.Bot;
using Telegram.Bot.Types;

namespace Eclipse.Core.Results;

public class EditTextResult : ResultBase
{
    public int MessageId { get; }

    public string Text { get; }

    public EditTextResult(int messageId, string text)
    {
        MessageId = messageId;
        Text = text;
    }

    public override async Task<Message?> SendAsync(ITelegramBotClient botClient, CancellationToken cancellationToken = default)
    {
        return await botClient.EditMessageTextAsync(ChatId, MessageId, Text, cancellationToken: cancellationToken);
    }
}
