﻿using Eclipse.Core.Attributes;
using Eclipse.Core.Core;
using Eclipse.Pipelines.Stores.Messages;
using Telegram.Bot.Types.ReplyMarkups;

namespace Eclipse.Pipelines.Pipelines.Daily;

[Route("", "/daily_morning")]
public class MorningPipeline : EclipsePipelineBase
{
    private readonly IMessageStore _messageStore;

    public MorningPipeline(IMessageStore messageStore)
    {
        _messageStore = messageStore;
    }

    protected override void Initialize()
    {
        RegisterStage(AskMood);
        RegisterStage(HandleChoice);
    }

    private IResult AskMood(MessageContext context)
    {
        var buttons = new InlineKeyboardButton[]
        {
            InlineKeyboardButton.WithCallbackData("👍"),
            InlineKeyboardButton.WithCallbackData("👎"),
        };

        var message = _messageStore.GetOrDefault(new MessageKey(context.ChatId));

        var menu = Menu(buttons, Localizer["Pipelines:Morning:AskMood"]);

        if (message is null || message.ReplyMarkup is null)
        {
            return menu;
        }

        var edit = Edit(message.MessageId, InlineKeyboardMarkup.Empty());

        return Multiple(menu, edit);
    }

    private IResult HandleChoice(MessageContext context)
    {
        var message = context.Value switch
        {
            "👍" => "Pipelines:Morning:GoodMood",
            "👎" => "Pipelines:Morning:BadMood",
            _ => "Pipelines:Morning:NotDefined"
        };

        return Text(Localizer[message]);
    }
}
