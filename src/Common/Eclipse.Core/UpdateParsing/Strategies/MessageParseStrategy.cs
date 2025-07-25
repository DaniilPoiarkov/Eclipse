﻿using Eclipse.Core.Context;

using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Eclipse.Core.UpdateParsing.Strategies;

internal sealed class MessageParseStrategy : IParseStrategy
{
    public UpdateType Type => UpdateType.Message;

    private readonly IServiceProvider _serviceProvider;

    public MessageParseStrategy(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public MessageContext? Parse(Update update)
    {
        var message = update.Message!;

        var chatId = message!.Chat.Id;
        var value = message.Text ?? string.Empty;
        var from = message.From!;

        var name = from.FirstName;
        var surname = from.LastName ?? string.Empty;

        var username = from.Username ?? string.Empty;

        var user = new TelegramUser(chatId, name, surname, username);

        return new MessageContext(chatId, value, user, _serviceProvider);
    }
}
