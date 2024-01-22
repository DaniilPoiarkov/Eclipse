using Eclipse.Core.Attributes;
using Eclipse.Core.Core;
using Eclipse.Pipelines.Stores.Messages;

using Telegram.Bot.Types;
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

        return EditedOrMenuResult(message, Localizer["Pipelines:Morning:AskMood"], buttons);
    }

    private IResult HandleChoice(MessageContext context)
    {
        var text = context.Value switch
        {
            "👍" => "Pipelines:Morning:GoodMood",
            "👎" => "Pipelines:Morning:BadMood",
            _ => "Pipelines:Morning:NotDefined"
        };

        var message = _messageStore.GetOrDefault(new MessageKey(context.ChatId));

        return EditedOrTextResult(message, Localizer[text]);
    }

    private static IResult EditedOrTextResult(Message? message, string text)
    {
        return EditedOrDefaultResult(message, Text(text));
    }

    private static IResult EditedOrMenuResult(Message? message, string text, InlineKeyboardButton[] buttons)
    {
        return EditedOrDefaultResult(message, Menu(buttons, text));
    }

    private static IResult EditedOrDefaultResult(Message? message, IResult @default)
    {
        return message is null || message.ReplyMarkup is null
            ? @default
            : Multiple(
                @default,
                Edit(message.MessageId, InlineKeyboardMarkup.Empty())
            );
    }
}
